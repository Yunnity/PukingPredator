using UnityEngine;

/// <summary>
/// An object that is supported by other objects.
/// If enough supports break, enable the physics in this object.
/// </summary>
public class SupportedObject : MonoBehaviour
{
    /// <summary>
    /// If this many supports fail, the structure will collapse
    /// </summary>
    [SerializeField]
    private int supportsBeforeCollapse = 1;

    private PhysicsEventListener physicsEventListener;



    private void Start()
    {
        physicsEventListener = GetComponent<PhysicsEventListener>();
    }



    public void ReduceSupportsByOne()
    {
        supportsBeforeCollapse--;
        if (supportsBeforeCollapse <= 0)
        {
            physicsEventListener.EnablePhysics();
        }
    }
}
