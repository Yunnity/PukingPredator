using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhysicsBehaviour))]
public class OneTimePlatform : MonoBehaviour
{
    /// <summary>
    /// How long it takes for the component to fall down.
    /// </summary>
    public float shakeDuration = 1.5f;

    /// <summary>
    /// How much the object can shake.
    /// </summary>
    public float shakeMagnitude = 0.05f;

    /// <summary>
    /// Used to lerp between different random vectors to smooth out shaking slightly.
    /// </summary>
    public float shakeSpeed = 0.15f;

    /// <summary>
    /// Used to check if the player is above the platform and grounded.
    /// </summary>
    [SerializeField]
    private CollisionTracker collisionTracker;

    private Consumable consumable;

    private PhysicsBehaviour pb;

    private bool hasBeenTriggered = false;



    void Start()
    {
        pb = GetComponent<PhysicsBehaviour>();

        consumable = GetComponent<Consumable>();
        if (consumable != null)
        {
            consumable.stateEvents[ItemState.beingConsumed].onEnter += OnFinish;
        }

        collisionTracker.SubscribeOnCollision(OnTrackerEnter);
    }



    private void OnTrackerEnter()
    {
        StartCoroutine(CheckForCollision());
    }

    private IEnumerator CheckForCollision()
    {
        var player = collisionTracker.collisions[0].GetComponent<Player>();

        while (collisionTracker.collisions.Count != 0)
        {
            if (player.movement.isGrounded)
            {
                StartCoroutine(Collapse());
                break;
            }

            yield return null;
        }
    }

    private IEnumerator Collapse()
    {
        if (hasBeenTriggered) { yield break; }
        hasBeenTriggered = true;

        var originalPosition = transform.position;
        var elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            transform.position = Vector3.Lerp(transform.position, originalPosition + RandomVector(), shakeSpeed);

            yield return null;
        }

        pb.EnablePhysics();
        OnFinish();
    }

    private void OnFinish()
    {
        StopAllCoroutines();
        consumable.stateEvents[ItemState.beingConsumed].onEnter -= OnFinish;
        Destroy(collisionTracker.gameObject);
        Destroy(this);
    }

    private Vector3 RandomVector()
    {
        return new Vector3(
            Random.Range(-shakeMagnitude, shakeMagnitude),
            0, //Random.Range(-shakeMagnitude, shakeMagnitude),
            Random.Range(-shakeMagnitude, shakeMagnitude)
        ); ;
    }
}
