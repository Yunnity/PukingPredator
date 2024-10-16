using System;
using UnityEngine;

/// <summary>
/// An event listener for enabling physics in an object.
/// </summary>
public class PhysicsEventListener : MonoBehaviour
{
    private bool isPhysicsEnabled = false;

    private Action onEnablePhysics;



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
        if (isPhysicsEnabled) { return; }
        isPhysicsEnabled = true;

        onEnablePhysics?.Invoke();
    }
}
