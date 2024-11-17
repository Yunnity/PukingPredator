using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component can be added to an object that is being supported by
/// another object OR to an object supporting another object, or both.
/// Labeling the support in both directions is allowed, but redundant.
/// The support component will automatically be added on the other object if
/// it is missing at runtime.
/// </summary>
[RequireComponent(typeof(PhysicsBehaviour))]
public class PhysicsSupport : MonoBehaviour
{
    /// <summary>
    /// List used purely to assign links in the level editor.
    /// </summary>
    [SerializeField]
    private List<GameObject> initiallySupportedBy = new();

    /// <summary>
    /// List used purely to assign links in the level editor.
    /// </summary>
    [SerializeField]
    public List<GameObject> initiallySupporting = new();

    private PhysicsBehaviour physicsObject;

    /// <summary>
    /// Other objects supporting this one.
    /// </summary>
    private List<PhysicsSupport> supportedBy = new();

    /// <summary>
    /// Other objects supported by this one.
    /// </summary>
    private List<PhysicsSupport> supporting = new();

    /// <summary>
    /// If this many supports fail, the structure will collapse
    /// </summary>
    [SerializeField]
    public int supportsBeforeCollapse { get; private set; } = 1;



    // Start is called before the first frame update
    void Start()
    {
        physicsObject = GetComponent<PhysicsBehaviour>();
        physicsObject.Subscribe(PhysicsEvent.onEnable, TriggerSupportFailure);

        //make sure all other objects have the correct components
        foreach (var other in initiallySupporting)
        {
            var otherSupport = other.GetComponent<PhysicsSupport>();
            if (otherSupport == null)
            {
                otherSupport = other.AddComponent<PhysicsSupport>();
            }
            AddSupportFor(otherSupport);
        }
        foreach (var other in initiallySupportedBy)
        {
            var otherSupport = other.GetComponent<PhysicsSupport>();
            if (otherSupport == null)
            {
                otherSupport = other.AddComponent<PhysicsSupport>();
            }
            AddSupportedBy(otherSupport);
        }
    }



    /// <summary>
    /// Adds component to the list of objects being supported by this one.
    /// </summary>
    /// <param name="other"></param>
    public void AddSupportFor(PhysicsSupport other)
    {
        other.AddSupportedBy(this);
    }
    /// <summary>
    /// Adds component to the list of objects supporting this one.
    /// </summary>
    /// <param name="other"></param>
    public void AddSupportedBy(PhysicsSupport other)
    {
        if (!supportedBy.Contains(other))
        {
            supportedBy.Add(other);
        }
        if (!other.supporting.Contains(this))
        {
            other.supporting.Add(this);
        }
    }
    /// <summary>
    /// Removes component from the list of objects being supported by this one.
    /// </summary>
    /// <param name="other"></param>
    public void RemoveSupportFor(PhysicsSupport other)
    {
        other.RemoveSupportedBy(this);
    }
    /// <summary>
    /// Removes component from the list of objects supporting this one.
    /// </summary>
    /// <param name="other"></param>
    public void RemoveSupportedBy(PhysicsSupport other)
    {
        other.supporting.Remove(this);
        supportedBy.Remove(other);
        supportsBeforeCollapse--;

        //if still supported, dont collapse
        if (supportsBeforeCollapse > 0) { return; }

        physicsObject.EnablePhysics();
    }

    private void TriggerSupportFailure()
    {
        List<PhysicsSupport> supportingCopy = new(supporting);
        foreach (PhysicsSupport other in supportingCopy)
        {
            if (other == null ) { continue; }
            RemoveSupportFor(other);
        }

        List<PhysicsSupport> supportedByCopy = new(supportedBy);
        foreach (PhysicsSupport other in supportedByCopy)
        {
            if (other == null) { continue; }
            RemoveSupportedBy(other);
        }

        physicsObject.Unsubscribe(PhysicsEvent.onEnable, TriggerSupportFailure);
    }
}
