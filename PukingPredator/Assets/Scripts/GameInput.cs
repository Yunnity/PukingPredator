using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private PlayerInput playerInput;
    public EventHandler onEatAction;
    public EventHandler onPukeAction;
    public EventHandler onResetLevelAction;

    public void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();

        playerInput.Player.Eat.performed += Eat_performed;
        playerInput.Player.Puke.performed += Puke_performed;
        playerInput.Player.Reset.performed += ResetLevel_performed;
    }

    private void Eat_performed(InputAction.CallbackContext obj)
    {
        onEatAction?.Invoke(this, EventArgs.Empty);
    }

    private void Puke_performed(InputAction.CallbackContext obj)
    {
        onPukeAction?.Invoke(this, EventArgs.Empty);
    }

    private void ResetLevel_performed(InputAction.CallbackContext obj)
    {
        onResetLevelAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetInputVectorNormalized()
    {
        Vector2 direction = playerInput.Player.Move.ReadValue<Vector2>();
        direction = direction.normalized;

        return direction;
    }
}
