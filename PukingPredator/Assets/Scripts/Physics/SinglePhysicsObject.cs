using UnityEngine;

/// <summary>
/// A single object that can enable physics.
/// </summary>
public class SinglePhysicsObject : MonoBehaviour
{
    private PhysicsEventListener physicsEventListener;

    private Rigidbody rb;




    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        physicsEventListener = GetComponent<PhysicsEventListener>();
        physicsEventListener.AddToListener(EnablePhysics);
    }



    public void EnablePhysics()
    {
        rb.isKinematic = false;
    }

}
