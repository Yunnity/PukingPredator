using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        foreach (Transform child in transform)
        {
            PhysicsEventListener c = child.GetComponent<PhysicsEventListener>();
            if (c != null)
            {
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
            SinglePhysicsObject c = child.GetComponent<SinglePhysicsObject>();
            if (c != null)
            {
                c.EnablePhysics();
            }
        }
        childrenPhysicsEnabled = true;
    }
}
