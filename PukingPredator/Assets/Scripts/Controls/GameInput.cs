using System;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    /// <summary>
    /// Triggers when the user presses the "eat" input.
    /// </summary>
    onEat,
    /// <summary>
    /// Triggers when the user presses the "jump" input.
    /// </summary>
    onJumpDown,
    /// <summary>
    /// Triggers when the user releases the "jump" input.
    /// </summary>
    onJumpUp,
    /// <summary>
    /// Triggers when the user releases the "jump" input.
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
    private Dictionary<EventType, Action> events = new Dictionary<EventType, Action>();

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
        foreach (EventType eventType in Enum.GetValues(typeof(EventType)))
        {
            events.Add(eventType, null);
        }
        controls.Player.Eat.performed += context => TriggerEvent(EventType.onEat);
        controls.Player.Jump.canceled += context => TriggerEvent(EventType.onJumpUp);
        controls.Player.Jump.performed += context => TriggerEvent(EventType.onJumpDown);
        controls.Player.Puke.performed += context => TriggerEvent(EventType.onPuke);
        controls.Player.Reset.performed += context => TriggerEvent(EventType.onResetLevel);
    }



    public void Subscribe(EventType eventType, Action action)
    {
        events[eventType] += action;
    }

    private void TriggerEvent(EventType eventType)
    {
        events[eventType]?.Invoke();
    }

    public void Unsubscribe(EventType eventType, Action action)
    {
        events[eventType] -= action;
    }
}
