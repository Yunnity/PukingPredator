using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class Puking : InputBehaviour
{
    /// <summary>
    /// The player's playerAnim component
    /// </summary>
    private PlayerAnimation anim;

    /// <summary>
    /// The players inventory.
    /// </summary>
    public Inventory inventory => player.inventory;

    /// <summary>
    /// The player component.
    /// </summary>
    private Player player;

    /// <summary>
    /// The prefab for the projectile blast.
    /// </summary>
    [SerializeField]
    private GameObject projectileBlastPrefab;

    /// <summary>
    /// The direction to puke in. Pukes forwards with a little force upwards.
    /// </summary>
    private Vector3 pukeDirection => transform.forward + Vector3.up * PUKE_VERTICAL_FACTOR;
    private const float PUKE_VERTICAL_FACTOR = 0.5f;

    /// <summary>
    /// The collision tracker used to roughly determine the objects that could
    /// be affected.
    /// </summary>
    [SerializeField]
    private CollisionTracker pukeExplodeCollisionTracker;

    /// <summary>
    /// The max distance if you fully charge the puke but had 2 items consumed.
    /// </summary>
    private const float PUKE_EXPLODE_BASE_MAX_DISTANCE = 2f;

    /// <summary>
    /// If pukeForce is greater than this, the puke will also knock back items
    /// in front of the player.
    /// </summary>
    private const float PUKE_EXPLODE_THRESHOLD = 0.5f;

    /// <summary>
    /// Force applied to object when puked. Depends on how long the puke button
    /// was held down for.
    /// </summary>
    private float pukeForce
    {
        get
        {
            float holdPercent = gameInput.pukeHoldDuration / MAX_PUKE_DURATION;
            holdPercent = Mathf.Pow(holdPercent, 2); //spend more time on lower charges
            holdPercent = Mathf.Clamp(holdPercent, 0, 1);
            return Mathf.Lerp(
                MIN_PUKE_FORCE * Mathf.Pow(player.relativeScale, 0.25f),
                MAX_PUKE_FORCE * Mathf.Pow(player.relativeScale, 0.35f),
                holdPercent
            );
        }
    }
    private const float MIN_PUKE_FORCE = 2f;
    private const float MAX_PUKE_FORCE = 10f;
    private const float MAX_PUKE_DURATION = 2f;

    /// <summary>
    /// The velocity that an object would be puked with if at this moment. Only
    /// meaningful while charging a puke.
    /// </summary>
    public Vector3 pukeVelocity => pukeDirection * pukeForce;



    void Start()
    {
        player = GetComponent<Player>();
        anim = player.GetComponent<PlayerAnimation>();

        Subscribe(InputEvent.onPuke, GameInput_Puke);
    }



    private void GameInput_Puke()
    {
        if (inventory.isEmpty) { return; }

        Consumable itemToPuke = inventory.PopItem();
        if (itemToPuke == null) return;

        itemToPuke.SetState(ItemState.beingPuked);

        anim.StartPukeAnim();

        bool pukeWithForce = pukeForce > MAX_PUKE_FORCE * PUKE_EXPLODE_THRESHOLD;
        AudioManager.Instance.PlaySFX(pukeWithForce ? AudioManager.ClipName.PukeForce : AudioManager.ClipName.Puking);

        Rigidbody itemRb = itemToPuke.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            //TODO: make this code work if there is no rigid body. perhaps just set
            //...the velocity and if there was a rigid body then we can reduce the velocity
            //...while calculating it based on the mass?
            //itemRb.AddForce(pukeVelocity * itemRb.mass, ForceMode.Impulse);
            itemRb.velocity = pukeVelocity;

            //if (pukeWithForce)
            //{
            //    KnockbackItemsInFrontofPlayer(pukeForce, itemRb);
            //    AddCollisionBlast(itemToPuke.gameObject, itemRb);
            //}
        }

    }

    /// <summary>
    /// Adds an object to cause collapses via the projectile.
    /// </summary>
    /// <param name="itemToPuke"></param>
    private void AddCollisionBlast(GameObject itemToPuke, Rigidbody itemRb)
    {
        if (projectileBlastPrefab == null) { return; }

        var projectileBlast = Instantiate(projectileBlastPrefab, inventory.transform.position, player.transform.rotation);
        projectileBlast.GetComponent<ProjectileBlast>().owner = itemToPuke;
        projectileBlast.GetComponent<FollowObject>().target = itemToPuke;
        projectileBlast.GetComponent<AlignWithVelocity>().rb = itemRb;
        projectileBlast.GetComponent<DestroyOnLowVelocity>().rb = itemRb;
    }

    /// <summary>
    /// Knocks back items in front of player
    /// </summary>
    /// <param name="pukeForce">0-1 puke force</param>
    /// <param name="itemRB">RigidBody of item being puked</param>
    public void KnockbackItemsInFrontofPlayer(float pukeForce, Rigidbody itemRB)
    {
        float percentCharged = pukeForce / MAX_PUKE_FORCE;
        var range = PUKE_EXPLODE_BASE_MAX_DISTANCE * percentCharged * player.relativeScale;

        foreach (var collision in pukeExplodeCollisionTracker.collisions)
        {
            if (collision.CompareTag(GameTag.player)) { continue; }

            var distance = Vector3.Distance(transform.position, collision.transform.position);
            if (distance > range) { continue; }

            Rigidbody hitRB = collision.GetComponent<Rigidbody>();
            if (hitRB == null || hitRB == itemRB) { continue; }

            PhysicsBehaviour pb = collision.GetComponent<PhysicsBehaviour>();
            if (pb != null)
            {
                pb.EnablePhysics();
            }

            hitRB.AddExplosionForce(1f * pukeForce, transform.position, 5f, 0.01f, ForceMode.VelocityChange);
        }
    }

}
