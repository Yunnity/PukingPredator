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
    public PlayerState state { get; private set; } = PlayerState.standing;
    public Dictionary<PlayerState, State> stateEvents = new();

    /// <summary>
    /// The speed that the player dashes towards an object (using lerp * deltatime).
    /// </summary>
    [SerializeField]
    private float eatingSpeed = 4f;

    /// <summary>
    /// The object being consumed.
    /// </summary>
    private GameObject eatingTarget;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;




    private void Awake()
    {
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            stateEvents.Add(state, new State());
        }

        stateEvents[PlayerState.eating].onUpdate += UpdateEating;

        //rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        stateEvents[state].onUpdate?.Invoke();
    }



    /// <summary>
    /// Makes the player dash towards the target.
    /// </summary>
    /// <param name="obj"></param>
    public void EatObject(GameObject obj)
    {
        //var distance = (transform.position - viewedObject.transform.position).magnitude;

        SetState(PlayerState.eating);
        eatingTarget = obj;
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
    private void UpdateEating()
    {
        var previousPosition = gameObject.transform.position;
        var targetPosition = eatingTarget.transform.position;
        gameObject.transform.position = Vector3.Lerp(previousPosition, targetPosition, eatingSpeed * Time.deltaTime);

        if (Vector3.Distance(gameObject.transform.position, targetPosition) < 0.1)
        {
            SetState(PlayerState.standing);
        }
    }
}