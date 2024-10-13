using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePhysicsObject : MonoBehaviour
{
    /*
     A single object that can enable physics
     */
    private Rigidbody rb;
    private PhysicsEventListener po;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        po = GetComponent<PhysicsEventListener>();

        po.onEnablePhysics += EnablePhysics;
    }

    public void EnablePhysics()
    {
        rb.isKinematic = false;
    }

}
