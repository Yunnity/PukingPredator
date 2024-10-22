using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A group of physics objects where the physics should be enabled
/// simultaneously if physics is enabled for any of the children.
/// </summary>
public class PhysicsCollapseGroup : MonoBehaviour
{
    private List<PhysicsBehaviour> relevantChildren;

    private bool hasTriggered = false;



    void Start()
    {
        //get the physics objects out of the descendants
        relevantChildren = gameObject
                            .GetDescendants()
                            .Select(c => c.GetComponent<PhysicsBehaviour>())
                            .Where(pb => pb != null)
                            .ToList();

        //sign up all children to cause the collapse if they get enabled
        foreach(var child in relevantChildren)
        {
            child.Subscribe(PhysicsEvent.onEnable, TriggerCollapse);
        }
    }



    private void TriggerCollapse()
    {
        if (hasTriggered) { return; }
        hasTriggered = true;

        foreach(var child in relevantChildren)
        {
            child.EnablePhysics();
        }
    }
}
