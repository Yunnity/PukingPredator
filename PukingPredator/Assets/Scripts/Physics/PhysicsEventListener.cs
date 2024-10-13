using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PhysicsEventListener : MonoBehaviour
{
    /*
     An event listener for enabling physics in an object
     */
    public Action onEnablePhysics;
}
