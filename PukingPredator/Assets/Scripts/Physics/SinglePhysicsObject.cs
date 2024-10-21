/// <summary>
/// A single object that can enable physics.
/// </summary>
public class SinglePhysicsObject : PhysicsBehaviour
{
    private void Start()
    {
        Subscribe(PhysicsEvent.onEnable, DisableIsKinematic);
    }



    public void DisableIsKinematic()
    {
        rb.isKinematic = false;
    }
}
