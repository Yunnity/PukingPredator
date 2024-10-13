using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    eating,
    standing
}

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private Dash dash;

    private Movement movement;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;

    public PlayerState state { get; private set; } = PlayerState.standing;
    public Dictionary<PlayerState, State> stateEvents = new();



    private void Awake()
    {
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            stateEvents.Add(state, new State());
        }

        rb = GetComponent<Rigidbody>();

        dash = GetComponent<Dash>();
        dash.onComplete += DashFinished;

        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        stateEvents[state].onUpdate?.Invoke();
    }



    private void DashFinished()
    {
        SetState(PlayerState.standing);
        movement.isManualMovementEnabled = true;
    }

    /// <summary>
    /// Makes the player dash towards the target.
    /// </summary>
    /// <param name="obj"></param>
    public void EatObject(GameObject obj)
    {
        var deltaPosition = obj.transform.position - transform.position;
        var distance = deltaPosition.magnitude;

        SetState(PlayerState.eating);
        movement.isManualMovementEnabled = false;
        dash.DashTo(obj);
    }

    public void SetState(PlayerState newState)
    {
        var previousState = state;
        if (newState == previousState) { return; }
        state = newState;
    }
}