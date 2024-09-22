using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float sensitivity;

    private Vector3 finalOffset;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player ??= GameObject.FindGameObjectsWithTag("Player")[0];
        finalOffset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        Rotate();
        transform.position = Vector3.Lerp(transform.position, player.transform.position + finalOffset, 0.25f);
        transform.LookAt(player.transform.position);
    }

    void Rotate()
    {
        finalOffset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * sensitivity, Vector3.up) * finalOffset;
        
    }
}
