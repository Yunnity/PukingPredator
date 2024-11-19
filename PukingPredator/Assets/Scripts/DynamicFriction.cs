using UnityEngine;

public class DynamicFriction : MonoBehaviour
{
    public PhysicMaterial groundMaterial;
    public PhysicMaterial airMaterial;

    private Collider objectCollider;

    private void Start()
    {
        objectCollider = GetComponent<Collider>();
    }

    public void SetFriction(bool isGrounded)
    {
        objectCollider.material = isGrounded ? groundMaterial : airMaterial;
    }
}
