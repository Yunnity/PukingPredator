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

    private PhysicsEventListener ownListener;

    // Start is called before the first frame update
    void Start()
    {
        ownListener = GetComponent<PhysicsEventListener>();
        ownListener.AddToListener(EnablePhysicsInChildren);

        // If this group is not being SUPPORTED, then enabling physics on one item in the group will cause physics to enable in all items.
        // Otherwise, we wait until this group is no long supported to enable physics in all children
        SupportedObject supportedObject = GetComponent<SupportedObject>();
        if (supportedObject == null)
        {
            AddListenersToChildren();
        }
    }

    private void EnablePhysicsInChildren()
    {
        foreach (Transform child in transform)
        {
            PhysicsEventListener c = child.GetComponent<PhysicsEventListener>();
            if (c != null)
            {
                c.EnablePhysics();
            }
        }
    }

    private void RemoveListenersFromChildren()
    {
        foreach (Transform child in transform)
        {
            PhysicsEventListener c = child.GetComponent<PhysicsEventListener>();
            if (c != null)
            {
                c.RemoveFromListener(InvokeSelf);
            }
        }
    }

    private void AddListenersToChildren()
    {
        foreach (Transform child in transform)
        {
            PhysicsEventListener c = child.GetComponent<PhysicsEventListener>();
            if (c != null)
            {
                c.AddToListener(InvokeSelf);
            }
        }
    }

    private void InvokeSelf()
    {
        RemoveListenersFromChildren();
        ownListener.EnablePhysics();
    }
}
