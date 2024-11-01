using UnityEngine;

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
    /// The layers that interactable objects can be on.
    /// </summary>
    [SerializeField]
    private LayerMask interactableLayers;

    private Player player;

    /// <summary>
    /// The object that the player is looking at.
    /// </summary>
    public Interactable targetInteractable { get; private set; } = null;



    private void Start()
    {
        player = GetComponent<Player>();
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

        if (previousTargetInteractable != null)
        {
            previousTargetInteractable.outline.enabled = false;
        }
        if (targetInteractable != null)
        {
            targetInteractable.outline.enabled = true;
        }
    }



    private Interactable GetViewedInteractable()
    {
        Interactable result = null;

        var multiplier = transform.localScale.y;
        var startBehindOffset = 0.1f * multiplier; //used to make objects directly in front of you get selected
        var maxDistance = baseRange * multiplier + startBehindOffset;
        var radius = baseRadius * multiplier;
        var ray = new Ray(
            transform.position + (radius+0.05f) * Vector3.up - transform.forward * startBehindOffset,
            transform.forward
        );
        RaycastHit hit;
        if (Physics.SphereCast(ray, radius, out hit, maxDistance, layerMask: interactableLayers))
        {
            var hitObject = hit.collider.gameObject;
            result = hitObject.GetComponent<Interactable>();
        }

        return result;
    }
}

