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
    /// The direction to puke in. Pukes forwards with a little force upwards.
    /// </summary>
    private Vector3 pukeDirection => transform.forward + Vector3.up * 0.1f;

    /// <summary>
    /// If pukeForce is greater than this, the puke will also knock back items in front of the player
    /// </summary>
    private const float PUKE_EXPLODE_THRESH = 0.8f;

    /// <summary>
    /// Force applied to object when puked. Depends on how long the puke button
    /// was held down for.
    /// </summary>
    private float pukeForce
    {
        get
        {
            float holdPercent = Mathf.Clamp(gameInput.pukeHoldDuration / MAX_PUKE_DURATION, 0, 1);
            return Mathf.Lerp(MIN_PUKE_FORCE * player.relativeScale, MAX_PUKE_FORCE, holdPercent);
        }
    }
    private const float MIN_PUKE_FORCE = 1f;
    private const float MAX_PUKE_FORCE = 20f;
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

        Rigidbody itemRb = itemToPuke.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            //TODO: make this code work if there is no rigid body. perhaps just set
            //...the velocity and if there was a rigid body then we can reduce the velocity
            //...while calculating it based on the mass?
            itemRb.AddForce(pukeVelocity * itemRb.mass, ForceMode.Impulse);

            // -1 because player loses an item when they puke
            if (pukeForce > MAX_PUKE_FORCE * PUKE_EXPLODE_THRESH)
            {
                KnockbackItemsInFrontofPlayer(pukeVelocity * pukeForce, pukeForce, itemRb);
            }
        }

    }

    public void KnockbackItemsInFrontofPlayer(Vector3 vec, float f, Rigidbody itemRB)
    {

        Vector3 halfExtents = new Vector3(1f, 0.5f, 0.5f); // Half the size of the box (x, y, z)

        // BoxCast in the direction of the velocity
        RaycastHit[] hits = Physics.BoxCastAll(transform.position + (transform.forward * 0.1f) + (transform.up * 0.6f), halfExtents, vec, Quaternion.identity, 3f);

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag(GameTag.player))
            {
                Rigidbody hitRB = hit.collider.GetComponent<Rigidbody>();
                PhysicsBehaviour pb = hit.collider.GetComponent<PhysicsBehaviour>();
                if (hitRB != null && pb != null && hitRB != itemRB)
                {
                    pb.EnablePhysics();
                    hitRB.AddExplosionForce(1f * f, transform.position, 5f, 0.01f, ForceMode.VelocityChange);
                    //hitRB.AddForce(vec * hitRB.mass * 0.5f, ForceMode.Impulse); // alternate force if you want to push things forward
                }
            }
        }
    }

}
