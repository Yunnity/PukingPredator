
public class MovableItem : PhysicsCollapseGroup
{
    protected override void Start()
    {
        base.Start();

        foreach (var child in relevantChildren)
        {
            child.Subscribe(PhysicsEvent.onEnable, DisableMovement);
        }
    }

    private void DisableMovement()
    {
        enabled = false;
    }
}
