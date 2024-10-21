using System;
using System.Collections.Generic;
using UnityEngine;

public enum PhysicsEvent
{
	onEnable,
	onHitHard,
}
/// <summary>
/// Base class for classes for physics object types.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class PhysicsBehaviour : MonoBehaviour
{
    /// <summary>
    /// Storage for the actions related to each event.
    /// </summary>
    private Dictionary<PhysicsEvent, Action> events = new();

    /// <summary>
    /// The percent of the velocity that is applied after a collision.
    /// </summary>
    private const float HARD_HIT_FORCE_RATIO = 0.3f;

    /// <summary>
    /// How much force it takes to count as being "hit hard"
    /// </summary>
    private const float HARD_HIT_IMPACT_THRESHOLD = 500f;

    private bool isPhysicsEnabled = false;

    protected Rigidbody rb;



    void Awake()
	{
        //Setup empty actions for every event type
        foreach (PhysicsEvent physicsEvent in Enum.GetValues(typeof(PhysicsEvent)))
        {
            events.Add(physicsEvent, null);
        }

        rb = GetComponent<Rigidbody>();

        Subscribe(PhysicsEvent.onHitHard, EnablePhysics);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    var otherRB = collision.gameObject.GetComponent<Rigidbody>();
    //    if (otherRB == null) { return; }

    //    //ignore collisions with the player, gravity would make it far too easy to proc on floors
    //    if (collision.gameObject.CompareTag(GameTag.player)) { return; }

    //    // Get the contact point's normal (assuming single point of contact for simplicity)
    //    Vector3 contactNormal = collision.contacts[0].normal;

    //    // Calculate the relative velocity between the colliding objects
    //    Vector3 relativeVelocity = collision.relativeVelocity;

    //    // Project the relative velocity onto the contact normal
    //    // This gives the velocity component in the direction of the collision
    //    float normalVelocity = Vector3.Dot(relativeVelocity, contactNormal);

    //    // If the normal velocity is positive, the objects are separating, so skip
    //    //if (normalVelocity > 0) { return; }

    //    float impactTime = Time.fixedDeltaTime;

    //    // Calculate the force of impact: F = m * dv / dt
    //    //float collisionForce = otherRB.mass * Mathf.Abs(normalVelocity) / impactTime;
    //    //TODO: ignoring mass for now since its not finely tuned, but it can be added later. should it?
    //    float collisionForce = Mathf.Abs(normalVelocity) / impactTime;

    //    // You can add custom behavior based on the force
    //    if (collisionForce > HARD_HIT_IMPACT_THRESHOLD)
    //    {
    //        //apply some of the force back to this object
    //        rb.AddForce(-1 * HARD_HIT_FORCE_RATIO * contactNormal * normalVelocity * otherRB.mass, ForceMode.Impulse);

    //        Debug.Log($"Significant impact detected! F = {collisionForce}N");
    //        TriggerEvent(PhysicsEvent.onHitHard);
    //    }
    //}



    public void EnablePhysics()
    {
        if (!isPhysicsEnabled)
        {
            TriggerEvent(PhysicsEvent.onEnable);
        }
    }

	public void Subscribe(PhysicsEvent physicsEvent, Action action)
	{
        events[physicsEvent] += action;
    }

    private void TriggerEvent(PhysicsEvent physicsEvent)
    {
        events[physicsEvent]?.Invoke();
    }

    public void Unsubscribe(PhysicsEvent physicsEvent, Action action)
    {
        events[physicsEvent] -= action;
    }
}

