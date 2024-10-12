using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerState
{
    eating,
    standing
}

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public PlayerState state { get; private set; } = PlayerState.standing;
    public Dictionary<PlayerState, State> stateEvents = new();

    [SerializeField]
    private float consumptionRate = 0.45f;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;

    private Vector3 target;

    private void Awake()
    {
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            stateEvents.Add(state, new State());
        }

        stateEvents[PlayerState.eating].onUpdate += Eating;
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

    public void SetState(PlayerState newState)
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
        if (Mathf.Abs(Vector3.Distance(gameObject.transform.position, target)) < 0.2) state = PlayerState.standing;
    }
}