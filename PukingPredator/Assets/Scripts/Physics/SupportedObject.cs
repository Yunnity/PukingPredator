using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportedObject : MonoBehaviour
{
    /*
     An object that is supported by other objects.
    If enough supports break, we enable the physics in this object.
     */

    [SerializeField]
    private int nSupportsThreshold; // if this many supports break, then we break this object

    private int nSupportsLeft;
    private bool broken = false;
    private PhysicsEventListener physicsEventListener;

    private void Start()
    {
        physicsEventListener = GetComponent<PhysicsEventListener>();
        nSupportsLeft = nSupportsThreshold;
    }

    public void ReduceSupportsByOne()
    {
        nSupportsLeft--;

        if (nSupportsLeft <= 0 && !broken)
        {
            broken = true;
            physicsEventListener.EnablePhysics();
        }
    }
}
