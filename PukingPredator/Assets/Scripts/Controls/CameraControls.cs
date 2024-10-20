using UnityEngine;

public class CameraControls : InputBehaviour
{
    /// <summary>
    /// Highest Y value for the rotation. This is how far up the camera can go
    /// above the player.
    /// </summary>
    private float maxY = 80;

    /// <summary>
    /// Lowest Y value for the rotation. This is how far down the camera can go
    /// below the player.
    /// </summary>
    private float minY = -80;

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
        var currentRotation = transform.localEulerAngles;

        var change = gameInput.cameraInput * sensitivity;
        currentRotation += new Vector3(-change.y, change.x, 0);
        currentRotation = ClampCircular(currentRotation);

        transform.localRotation = Quaternion.Euler(currentRotation);
    }



    /// <summary>
    /// Deals with the wrap around caused by swapping from 0-360 degrees
    /// instantly when going down.
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    private Vector3 ClampCircular(Vector3 rotation)
    {
        if (rotation.x > 180) { rotation.x -= 360; }
        rotation.x = Mathf.Clamp(rotation.x, minY, maxY);
        if (rotation.x < 0) { rotation.x += 360; }
        return rotation;
    }
}
