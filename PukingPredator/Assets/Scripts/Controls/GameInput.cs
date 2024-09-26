using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    /// <summary>
    /// The component that looks for control inputs.
    /// </summary>
    private PlayerControls controls;

    /// <summary>
    /// The movement input vector. Has a max magnitude of 1.
    /// </summary>
    public Vector2 movementInput => controls.Player.Move.ReadValue<Vector2>();

    /// <summary>
    /// Triggers when the user presses the "eat" input.
    /// </summary>
    public event Action onEat;

    /// <summary>
    /// Triggers when the user presses the "jump" input.
    /// </summary>
    public event Action onJumpDown;

    /// <summary>
    /// Triggers when the user releases the "jump" input.
    /// </summary>
    public event Action onJumpUp;

    /// <summary>
    /// Triggers when the user presses the "puke" input.
    /// </summary>
    public event Action onPuke;

    /// <summary>
    /// Triggers when the user presses the "reset level" input.
    /// </summary>
    public event Action onResetLevel;



    public void Awake()
    {
        controls = new();
        controls.Player.Enable();

        controls.Player.Eat.performed += PerformEat;

        controls.Player.Jump.canceled += CancelJump;
        controls.Player.Jump.performed += PerformJump;

        controls.Player.Puke.performed += PerformPuke;

        controls.Player.Reset.performed += PerformResetLevel;
    }



    private void CancelJump(InputAction.CallbackContext obj)
    {
        onJumpUp?.Invoke();
    }

    private void PerformEat(InputAction.CallbackContext obj)
    {
        onEat?.Invoke();
    }

    private void PerformJump(InputAction.CallbackContext obj)
    {
        onJumpDown?.Invoke();
    }

    private void PerformPuke(InputAction.CallbackContext obj)
    {
        onPuke?.Invoke();
    }

    private void PerformResetLevel(InputAction.CallbackContext obj)
    {
        onResetLevel?.Invoke();
    }
}
