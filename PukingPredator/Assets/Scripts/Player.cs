using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum MovementState
{
    eating,
    standing
}

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public MovementState state { get; private set; } = MovementState.standing;
    public Dictionary<MovementState, PlayerState> stateEvents = new();

    [SerializeField]
    private float consumptionRate = 0.45f;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;

    private Vector3 target;

    private void Awake()
    {
        foreach (MovementState moveState in Enum.GetValues(typeof(MovementState)))
        {
            stateEvents.Add(moveState, new PlayerState());
        }

        stateEvents[MovementState.eating].onUpdate += Eating;
        //rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        stateEvents[state].onUpdate?.Invoke();
    }

    /// <summary>
    /// Sets the target to dash towards
    /// </summary>
    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }

    public void SetState(MovementState newState)
    {
        var previousState = state;
        if (newState == previousState) { return; }
        state = newState;
    }

    /// <summary>
    /// Creates the dash effect bringing the player close to the consumed object
    /// </summary>
    public void Eating()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, target, consumptionRate);
        if (Mathf.Abs(Vector3.Distance(gameObject.transform.position, target)) < 0.2) state = MovementState.standing;
    }
}