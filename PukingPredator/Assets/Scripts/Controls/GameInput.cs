using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// The events that occur based on user inputs.
/// </summary>
public enum InputEvent
{
    /// <summary>
    /// Triggers when a major or minor device swap occurs
    /// (ie keyboard+mouse -> gamepad or keyboard -> mouse).
    /// </summary>
    onDeviceSwapAny,
    /// <summary>
    /// Triggers when a major device swap occurs (ie keyboard+mouse -> gamepad,
    /// but not keyboard -> mouse).
    /// </summary>
    onDeviceSwapMajor,
    /// <summary>
    /// Triggers when the user releases the "eat" input.
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
    /// Triggers when the user presses the "puke" input.
    /// </summary>
    onPukeStart,
    /// <summary>
    /// Triggers when the user releases the "puke" input.
    /// </summary>
    onPuke,
    /// <summary>
    /// Triggers when the user cancels the "puke", generally by going to a menu.
    /// </summary>
    onPukeCancel,
    /// <summary>
    /// Triggers when the user presses the "reset level" input.
    /// </summary>
    onResetLevel,
    /// <summary>
    /// Triggers when the user presses the "pause" input
    /// </summary>
    onPause,
    /// <summary>
    /// Triggers when the user presses the "eat" input
    /// </summary>
    onAim,
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
    public PlayerControls controls { get; private set; }

    /// <summary>
    /// Storage for the actions related to each event.
    /// </summary>
    private Dictionary<InputEvent, Action> events = new();

    public GamepadType gamepadType { get; private set; }

    private bool isChargingPuke = false;

    /// <summary>
    /// The most recently used input device type. Can change constantly in game.
    /// </summary>
    public InputDeviceType inputDeviceType { get; private set; } = InputDeviceType.mouse;

    /// <summary>
    /// The minimum time for something to be held for. A tap will still be
    /// registered as this much time.
    /// </summary>
    public const float minHoldTime = 0.1f;

    /// <summary>
    /// The movement input vector. Has a max magnitude of 1.
    /// </summary>
    public Vector2 movementInput => controls.Player.Move.ReadValue<Vector2>();

    /// <summary>
    /// Used to track when the button was first pressed to determine how long
    /// it was held.
    /// </summary>
    private float buttonPressTime = 0f;

    /// <summary>
    /// The amount of time that the puke button has been held. Only meaningful
    /// at the time that the puke event is triggered.
    /// </summary>
    public float pukeHoldDuration => Mathf.Max(0, Time.time - buttonPressTime - minHoldTime);

    /// <summary>
    /// The amount of time that the eat button has been held. Only meaningful
    /// at the time that the eat event is triggered.
    /// </summary>
    public float eatHoldDuration => Mathf.Max(0, Time.time - buttonPressTime - minHoldTime);

    protected override void Awake()
    {
        base.Awake();

        InputSystem.onEvent += DetectDeviceChange;

        controls = new();
        controls.Player.Enable();

        //Setup empty actions for every event type
        foreach (InputEvent inputEvent in Enum.GetValues(typeof(InputEvent)))
        {
            events.Add(inputEvent, null);
        }
        controls.Player.Eat.performed += context =>
        {
            buttonPressTime = Time.time;
            TriggerEvent(InputEvent.onAim);
        };
        controls.Player.Eat.canceled += context => TriggerEvent(InputEvent.onEat);

        controls.Player.Jump.canceled += context => TriggerEvent(InputEvent.onJumpUp);
        controls.Player.Jump.performed += context => TriggerEvent(InputEvent.onJumpDown);

        controls.Player.Puke.canceled += context =>
        {
            if (!isChargingPuke) { return; }
            TriggerEvent(InputEvent.onPuke);
        };
        controls.Player.Puke.performed += context =>
        {
            isChargingPuke = true;
            buttonPressTime = Time.time;
            TriggerEvent(InputEvent.onPukeStart);
        };

        controls.Player.Pause.performed += context =>
        {
            if (isChargingPuke)
            {
                isChargingPuke = false;
                TriggerEvent(InputEvent.onPukeCancel);
            }
        };
        controls.Player.Pause.performed += context => TriggerEvent(InputEvent.onPause);
    }



    private void DetectDeviceChange(InputEventPtr eventPtr, InputDevice device)
    {
        var initialDeviceType = inputDeviceType;

        // Check if the event is a state change (e.g., button press, move event)
        if (eventPtr.IsA<StateEvent>() || eventPtr.IsA<DeltaStateEvent>())
        {
            // Ignore mouse position changes to reduce noise
            if (device is Mouse && eventPtr.IsA<DeltaStateEvent>())
                return;

            // Get the type of input device generating the event
            if (device is Keyboard)
            {
                inputDeviceType = InputDeviceType.keyboard;
            }
            else if (device is Mouse)
            {
                inputDeviceType = InputDeviceType.mouse;
            }
            else if (device is Gamepad gamepad)
            {
                inputDeviceType = InputDeviceType.gamepad;

                if (gamepad is DualShockGamepad)
                {
                    gamepadType = GamepadType.playStation;
                }
                else if (gamepad.description.manufacturer.ToLower().Contains("microsoft"))
                {
                    gamepadType = GamepadType.xbox;
                }
                else
                {
                    gamepadType = GamepadType.other;
                }
            }
            else
            {
                inputDeviceType = InputDeviceType.other;
            }
        }

        if (initialDeviceType == inputDeviceType) { return; }

        TriggerEvent(InputEvent.onDeviceSwapAny);

        //if either wasnt keyboard/mouse, it must be a major change
        if (!initialDeviceType.IsKeyboardOrMouse() || !inputDeviceType.IsKeyboardOrMouse())
        {
            TriggerEvent(InputEvent.onDeviceSwapMajor);
        }
    }

    public void Subscribe(InputEvent inputEvent, Action action)
    {
        events[inputEvent] += action;
    }

    private void TriggerEvent(InputEvent inputEvent)
    {
        if (!GameManager.isGamePaused || (GameManager.isGamePaused && inputEvent == InputEvent.onPause))
        {
            events[inputEvent]?.Invoke();
        }
    }

    public void Unsubscribe(InputEvent inputEvent, Action action)
    {
        events[inputEvent] -= action;
    }
}
