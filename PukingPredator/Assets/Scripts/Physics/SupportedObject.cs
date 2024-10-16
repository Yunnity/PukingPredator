using UnityEngine;

/// <summary>
/// An object that is supported by other objects.
/// If enough supports break, enable the physics in this object.
/// </summary>
public class SupportedObject : MonoBehaviour
{
    /// <summary>
    /// How many supports can fail without a collapse.
    /// </summary>
    [SerializeField]
    private int acceptableSupportFailures = 0;

    private bool hasCollapsed = false;

    private PhysicsEventListener physicsEventListener;



    private void Start()
    {
        physicsEventListener = GetComponent<PhysicsEventListener>();
    }



    public void ReduceSupportsByOne()
    {
        if (acceptableSupportFailures > 0)
        {
            acceptableSupportFailures--;
            return;
        }

        if (hasCollapsed) { return; }
        hasCollapsed = true;

        physicsEventListener.EnablePhysics();
    }
}
