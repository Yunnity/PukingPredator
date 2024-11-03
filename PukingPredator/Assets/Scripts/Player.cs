using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    dashing,
    standing
}

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private InteractablePicker interactablePicker;

    [SerializeField]
    private Inventory _inventory;
    public Inventory inventory => _inventory;

    public Movement movement { get; private set; }

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;

    public PlayerState state { get; private set; } = PlayerState.standing;
    public Dictionary<PlayerState, State> stateEvents = new();

    /// <summary>
    /// The object that the player is looking at.
    /// </summary>
    public Interactable targetInteractable => interactablePicker.targetInteractable;



    private void Awake()
    {
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            stateEvents.Add(state, new State());
        }

        rb = GetComponent<Rigidbody>();

        movement = GetComponent<Movement>();

        interactablePicker = GetComponent<InteractablePicker>();

        AudioManager.Instance.PlayBackground();

        Vector3? spawnPosition = CheckpointManager.Instance.LastCheckpointPosition;
        if (spawnPosition != null) transform.position = (Vector3)spawnPosition;
        CheckpointManager.Instance.ResetCollected();
    }

    private void Update()
    {
        stateEvents[state].onUpdate?.Invoke();
    }



    public void SetState(PlayerState newState)
    {
        var previousState = state;
        if (newState == previousState) { return; }
        state = newState;
    }
}