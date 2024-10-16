using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class CameraControls : MonoBehaviour
{

    public Vector2 turn;
    private float sens = 6f;
    private float maxY = 40;
    private float minY = -40;
    public float speed = 1;
    public Transform _transform;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        turn.x += Input.GetAxis("Mouse X") * sens;
        turn.y += Input.GetAxis("Mouse Y") * sens;
        if (turn.y < minY)
        {
            turn.y = minY;
        }
        else if (turn.y > maxY)
        {
            turn.y = maxY;
        }
        _transform.rotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }
}
