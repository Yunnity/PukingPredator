using UnityEngine;

public class RotatingItem : MovableItem
{
    [SerializeField]
    private float rotationSpeed = 100f;

    [SerializeField]
    private Vector3 rotationAxis = Vector3.up;

    private void Update()
    {
        transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime);
    }
}