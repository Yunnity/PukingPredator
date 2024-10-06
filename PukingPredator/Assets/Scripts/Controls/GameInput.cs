using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The events that occur based on user inputs.
/// </summary>
public enum InputEvent
{
    /// <summary>
    /// Triggers when the user presses the "eat" input.
    /// </summary>
    onEat,
    /// <summary>
    /// Triggers when the user presses the "mouse 0" input.
    /// </summary>
    onJumpDown,
    /// <summary>
    /// Triggers when the user releases the "jump" input.
    /// </summary>
    onJumpUp,
    /// <summary>
    /// Triggers when the user releases the "mouse 1" input.
    /// </summary>
    onPuke,
    /// <summary>
    /// Triggers when the user presses the "reset level" input.
    /// </summary>
    onResetLevel,
}

public class GameInput : SingletonMonobehaviour<GameInput>
{
    /// <summary>
    /// The component that looks for control inputs.
    /// </summary>
    private PlayerControls controls;

    /// <summary>
    /// Storage for the actions related to each event.
    /// </summary>
    private Dictionary<InputEvent, Action> events = new();

    /// <summary>
    /// The movement input vector. Has a max magnitude of 1.
    /// </summary>
    public Vector2 movementInput => controls.Player.Move.ReadValue<Vector2>();



    protected override void Awake()
    {
        base.Awake();

        controls = new();
        controls.Player.Enable();

        //Setup empty actions for every event type
        foreach (InputEvent inputEvent in Enum.GetValues(typeof(InputEvent)))
        {
            events.Add(inputEvent, null);
        }
        controls.Player.Eat.performed += context => TriggerEvent(InputEvent.onEat);
        controls.Player.Jump.canceled += context => TriggerEvent(InputEvent.onJumpUp);
        controls.Player.Jump.performed += context => TriggerEvent(InputEvent.onJumpDown);
        controls.Player.Puke.performed += context => TriggerEvent(InputEvent.onPuke);
        controls.Player.Reset.performed += context => TriggerEvent(InputEvent.onResetLevel);
    }



    public void Subscribe(InputEvent inputEvent, Action action)
    {
        events[inputEvent] += action;
    }

    private void TriggerEvent(InputEvent inputEvent)
    {
        events[inputEvent]?.Invoke();
    }

    public void Unsubscribe(InputEvent inputEvent, Action action)
    {
        events[inputEvent] -= action;
    }
}
