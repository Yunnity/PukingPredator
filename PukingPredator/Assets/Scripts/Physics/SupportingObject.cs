using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportingObject : MonoBehaviour
{
    /*
     An object that may be supporting multiple other objects. It notifies all supported objects if its physics gets enabled.
     */

    [SerializeField]
    private GameObject[] supportedObjects;

    private PhysicsEventListener eventListener;

    private bool attemptedUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        eventListener = GetComponent<PhysicsEventListener>();
        eventListener.onEnablePhysics += UpdateSupportedObjects;
    }

    private void UpdateSupportedObjects()
    {
        foreach (GameObject obj in supportedObjects)
        {
            SupportedObject so = obj.GetComponent<SupportedObject>();
            if (so != null)
            {
                so.ReduceSupportsByOne();
            }
        }
        eventListener.onEnablePhysics -= UpdateSupportedObjects;
    }
}