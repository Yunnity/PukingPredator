using UnityEngine;

public class FollowPlayer : FollowObject
{
    protected override void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectsWithTag("Player")[0];
        }

        base.Start();
    }
}
