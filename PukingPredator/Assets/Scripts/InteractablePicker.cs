using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Player))]
public class InteractablePicker : MonoBehaviour
{
    /// <summary>
    /// The radius of spherecasts when the players scale is 1.
    /// </summary>
    private float baseRadius = 0.2f;

    /// <summary>
    /// The max distance when the players scale is 1.
    /// </summary>
    private float baseRange = 2f;

    /// <summary>
    /// Used if raycast fails.
    /// </summary>
    [SerializeField]
    private CollisionTracker collisionTracker;

    /// <summary>
    /// The layers that interactable objects can be on.
    /// </summary>
    private LayerMask interactableLayers;

    private Player player;

    /// <summary>
    /// The object that the player is looking at.
    /// </summary>
    public Interactable targetInteractable { get; private set; } = null;

    /// <summary>
    /// The list of objects that are being supported to the target
    /// </summary>
    public List<Interactable> targetSupports { get; private set; } = null;

    private void Start()
    {
        targetSupports = new List<Interactable>();
        player = GetComponent<Player>();
        interactableLayers = GameLayer.GetLayerMask(collisionTracker.gameObject.layer);
    }

    private void Update()
    {
        var previousTargetInteractable = targetInteractable;
        targetInteractable = GetViewedInteractable();

        //dont let the player select edible things while full
        if (player.inventory.isFull && targetInteractable is Consumable)
        {
            targetInteractable = null;
        }

        if (!targetInteractable || previousTargetInteractable != targetInteractable)
        {
            foreach (Interactable target in targetSupports)
            {
                target.ChangeColor(Color.white);
                setOutline(target, false);
                targetSupports = new List<Interactable>();
            }
        }
        

        if (previousTargetInteractable != null)
        {
            setOutline(previousTargetInteractable, false);
        }

        if (targetInteractable != null)
        {
            setOutline(targetInteractable, true);
        }
    }



    private Interactable GetViewedInteractable()
    {
        Interactable result = null;
        RaycastHit hit;

        var forward = transform.forward;

        //first try a simple raycast
        if (RaycastInDirection(direction: forward, out hit))
        {
            var hitObject = hit.collider.gameObject;
            result = hitObject.GetComponent<Interactable>();
            if (result != null) { return result; }
        }

        //that failed, check for objects in the collision tracker
        float smallestAngle = float.MaxValue;
        Vector3 bestDirection = Vector3.zero;
        foreach (var collision in collisionTracker.collisions)
        {
            var interactable = collision.GetComponent<Interactable>();
            if (interactable == null) { continue; }

            // Calculate the direction to the collision point, projected onto the XZ plane
            Vector3 directionToCollision = collision.transform.position - transform.position;
            Vector3 directionToCollisionXZ = new Vector3(directionToCollision.x, 0, directionToCollision.z).normalized;

            // Calculate the angle between transform.forward and the direction to the collision
            float angle = Vector3.Angle(forward, directionToCollisionXZ);

            // Update if this is the smallest angle found
            if (angle < smallestAngle)
            {
                smallestAngle = angle;
                bestDirection = directionToCollisionXZ;
                result = interactable;
            }
        }

        if (result == null) { return null; }

        //raycast again to make sure you get the closest thing at the smallest
        //angle, not just the smallest angle
        if (RaycastInDirection(direction: bestDirection, out hit))
        {
            var hitObject = hit.collider.gameObject;
            result = hitObject.GetComponent<Interactable>();
        }

        return result;
    }

    private bool RaycastInDirection(Vector3 direction, out RaycastHit hit)
    {
        var multiplier = transform.localScale.y;
        var startBehindOffset = 0.1f * multiplier; //used to make objects directly in front of you get selected
        var maxDistance = baseRange * multiplier + startBehindOffset;
        var radius = baseRadius * multiplier;
        var ray = new Ray(
            transform.position + (radius + 0.05f) * Vector3.up - transform.forward * startBehindOffset,
            direction
        );
        return Physics.SphereCast(ray, radius, out hit, maxDistance, layerMask: interactableLayers);
    }

    private void setOutline(Interactable interactable, bool enabled)
    {
        if (interactable.outline.enabled == enabled) return;

        interactable.outline.enabled = enabled;
        Color supportColor = enabled ? Color.red : Color.white;
        // Enables group highlight for supported objects
        PhysicsSupport support = interactable.GetComponent<PhysicsSupport>();
        if (support != null)
        {
            if (support.supportsBeforeCollapse != 1) return;

            foreach (var other in support.initiallySupporting)
            {
                // Ensures fallen walls are not highlihgted if the base is highlighted
                if (other.GetComponent<PhysicsSupport>().supportsBeforeCollapse != 1) continue;
                Interactable supportInteractable = other.GetComponent<Interactable>();

                setOutline(supportInteractable, enabled);
                supportInteractable.ChangeColor(supportColor);
                if (enabled) targetSupports.Add(supportInteractable);
            }
        }

        // Hihglights objects in the same physics collapse group
        Transform parentTransform = interactable.transform.parent;
        PhysicsCollapseGroup physicsCollapseGroup = parentTransform.GetComponent<PhysicsCollapseGroup>();

        if (physicsCollapseGroup != null)
        {
            foreach (Transform child in parentTransform)
            {
                if (child == interactable.transform) continue;
                Interactable supportInteractable = child.GetComponent<Interactable>();
                if (supportInteractable != null)
                {
                    supportInteractable.outline.enabled = enabled;
                    supportInteractable.ChangeColor(supportColor);
                    if (enabled) targetSupports.Add(supportInteractable);
                }
            }
        }
    }
}
