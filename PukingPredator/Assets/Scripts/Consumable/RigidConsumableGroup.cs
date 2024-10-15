using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

public class RigidConsumableGroup : MonoBehaviour
{
    /*
     Put consumable objects with isKinematic=true into a group, put this script on the group
     Eating one item out of the group will enable physics for all items in the group
    */

    private bool childrenPhysicsDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        disablePhysicsInChildren();
    }

    /// <summary>
    /// enable physics in all children if it was disabled
    /// </summary>
    public void enablePhysicsInChildren()
    {
        if (!childrenPhysicsDisabled)
        {
            return;
        }

        foreach (Transform child in transform)
        {
            Consumable c = child.GetComponent<Consumable>();
            if (c != null)
            {
                c.SetRBKinematic(false);
            }
        }
        childrenPhysicsDisabled = false;
    }

    /// <summary>
    /// disable physics in all children
    /// </summary>
    private void disablePhysicsInChildren()
    {
        foreach (Transform child in transform)
        {
            Consumable c = child.GetComponent<Consumable>();
            if (c != null)
            {
                c.SetRBKinematic(true);
                c.stateEvents[ItemState.beingConsumed].onUpdate += enablePhysicsInChildren;
            }
        }
        childrenPhysicsDisabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
