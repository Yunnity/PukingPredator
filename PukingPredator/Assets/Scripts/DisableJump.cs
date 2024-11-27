using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class DisableJump : MonoBehaviour
{
    private void Start()
    {
        GameInput.Instance.controls.Player.Jump.Disable();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameTag.player)
            GameInput.Instance.controls.Player.Jump.Enable();
    }
}
