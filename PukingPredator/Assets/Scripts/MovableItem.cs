using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovableItem : PhysicsCollapseGroup
{
    protected override void Start()
    {
        base.Start();

        foreach (var child in relevantChildren)
        {
            child.Subscribe(PhysicsEvent.onEnable, disableMovement);
        }
    }

    private void disableMovement()
    {
        enabled = false;
    }
}
