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
    /// Triggers when the user presses the "jump" input.
    /// </summary>
    onJumpDown,
    /// <summary>
    /// Triggers when the user releases the "jump" input.
    /// </summary>
    onJumpUp,
    /// <summary>
    /// Triggers when the user releases the "puke" input.
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
    /// The input vector for the camera. Has a max magnitude of 1.
    /// </summary>
    public Vector2 cameraInput => controls.Player.Look.ReadValue<Vector2>();

    /// <summary>
    /// The component that looks for control inputs.
    /// </summary>
    private PlayerControls controls;

    /// <summary>
    /// Storage for the actions related to each event.
    /// </summary>
    private Dictionary<InputEvent, Action> events = new();

    /// <summary>
    /// The minimum time for something to be held for. A tap will still be
    /// registered as this much time.
    /// </summary>
    private float minHoldTime = 0.1f;

    /// <summary>
    /// The movement input vector. Has a max magnitude of 1.
    /// </summary>
    public Vector2 movementInput => controls.Player.Move.ReadValue<Vector2>();

    /// <summary>
    /// Used to track when the button was first pressed to determine how long
    /// it was held.
    /// </summary>
    private float pukePressTime = 0f;

    /// <summary>
    /// The amount of time that the puke button has been held. Only meaningful
    /// at the time that the puke event is triggered.
    /// </summary>
    public float pukeHoldDuration { get; private set; } = 0f;



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

        controls.Player.Puke.canceled += context =>
        {
            pukeHoldDuration = Mathf.Max(minHoldTime, Time.time - pukePressTime);
            TriggerEvent(InputEvent.onPuke);
        };
        controls.Player.Puke.performed += context => pukePressTime = Time.time;

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
