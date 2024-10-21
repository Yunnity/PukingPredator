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
    }



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

