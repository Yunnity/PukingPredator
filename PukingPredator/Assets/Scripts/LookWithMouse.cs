using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class LookWithMouse : MonoBehaviour
{

    public Vector2 turn;
    private float sens = 10f;
    private float maxY = 30;
    private float minY = -20;
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
        Debug.Log(-turn.y);
        _transform.rotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }
}