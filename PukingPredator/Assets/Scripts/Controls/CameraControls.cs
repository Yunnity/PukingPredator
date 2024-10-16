using UnityEngine;

public class CameraControls : InputBehaviour
{
    /// <summary>
    /// Highest Y value for the rotation. This is how far down the camera can go
    /// below the player.
    /// </summary>
    private float maxY = 34;

    /// <summary>
    /// Lowest Y value for the rotation. This is how far up the camera can go
    /// above the player.
    /// </summary>
    private float minY = -40;

    /// <summary>
    /// The current camera rotation.
    /// </summary>
    private Vector2 rotation;

    /// <summary>
    /// The sensitivity of the camera movement.
    /// </summary>
    private float sensitivity = 1f;



    void Start()
    {
        //lock the mouse to the center of the view
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        rotation += gameInput.cameraInput * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, minY, maxY);

        transform.rotation = Quaternion.Euler(-rotation.y, rotation.x, 0);
    }
}
