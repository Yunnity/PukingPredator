using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that may be supporting multiple other objects. It notifies all
/// supported objects if its physics gets enabled.
/// </summary>
public class SupportingObject : MonoBehaviour
{
    private PhysicsEventListener physicsEventListener;

    /// <summary>
    /// Other objects supported by this one.
    /// </summary>
    [SerializeField]
    private List<GameObject> supportedObjects;



    // Start is called before the first frame update
    void Start()
    {
        physicsEventListener = GetComponent<PhysicsEventListener>();
        physicsEventListener.AddToListener(UpdateSupportedObjects);
    }



    private void UpdateSupportedObjects()
    {
        foreach (GameObject obj in supportedObjects)
        {
            SupportedObject so = obj.GetComponent<SupportedObject>();
            if (so != null)
            {
                so.ReduceSupportsByOne();
            }
        }
        physicsEventListener.RemoveFromListener(UpdateSupportedObjects);
    }
}
