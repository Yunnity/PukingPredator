using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GroupPhysicsObject : MonoBehaviour
{
    /*
     A group of SinglePhysicsObjects where the physics should be enabled simultaneously 
    if physics are enabled in any of the children
     */
    private PhysicsEventListener po;
    private bool childrenPhysicsEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        PhysicsEventListener ownListener = GetComponent<PhysicsEventListener>();
        ownListener.AddToListener(EnablePhysicsInChildren);
        foreach (Transform child in transform)
        {
            PhysicsEventListener c = child.GetComponent<PhysicsEventListener>();
            if (c != null)
            {
                Debug.Log("Added EnablePhysicsInChildren");
                c.AddToListener(EnablePhysicsInChildren);
            }
        }
    }

    private void EnablePhysicsInChildren()
    {
        // maybe we don't need this check
        if (childrenPhysicsEnabled)
        {
            return;
        }
        foreach (Transform child in transform)
        {
            PhysicsEventListener c = child.GetComponent<PhysicsEventListener>();
            if (c != null)
            {
                c.RemoveFromListener(EnablePhysicsInChildren); // cleanup
                c.EnablePhysics();
            }
        }
        childrenPhysicsEnabled = true;
    }
}
