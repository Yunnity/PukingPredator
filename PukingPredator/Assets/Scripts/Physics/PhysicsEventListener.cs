using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEventListener : MonoBehaviour
{
    /*
     An event listener for enabling physics in an object
     */
    private Action onEnablePhysics;

    private bool physicsEnabled = false;

    public void AddToListener(Action item)
    {
        onEnablePhysics += item;
    }

    public void RemoveFromListener(Action item)
    {
        onEnablePhysics -= item;
    }

    public void EnablePhysics()
    {
        if (!physicsEnabled)
        {
            onEnablePhysics?.Invoke();
            physicsEnabled = true;
        }
    }
}
