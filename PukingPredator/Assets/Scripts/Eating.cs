using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Player))]
public class Eating : InputBehaviour
{
    /// <summary>
    /// The mass of the player before having eaten anything.
    /// </summary>
    private float baseMass;

    /// <summary>
    /// The layers that consumable objects can be on.
    /// </summary>
    [SerializeField]
    private LayerMask consumableLayers;

    /// <summary>
    /// The layers that dashable objects can be on.
    /// </summary>
    [SerializeField]
    private LayerMask dashLayers;

    /// <summary>
    /// The players inventory.
    /// </summary>
    [SerializeField]
    private Inventory inventory;

    /// <summary>
    /// The object that the player is looking at to eat.
    /// </summary>
    private Consumable viewedConsumable = null;

    /// <summary>
    /// The player component.
    /// </summary>
    Player player;

    /// <summary>
    /// The distance that items spawn ahead of the player when puking. (TEMP)
    /// </summary>
    private float pukeDistance = 0.4f;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// The scale factor for the player's mass as they eat objects
    /// </summary>
    private const float MASSFACTOR = 0.05f;

    private Vector3 baseScale;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();

        baseMass = rb.mass;

        baseScale = gameObject.transform.localScale;

        inventory.onChange += UpdateMass;

        Subscribe(InputEvent.onEat, GameInput_Eat);
        Subscribe(InputEvent.onPuke, GameInput_Puke);
    }

    private void Update()
    {
        var previousViewedConsumable = viewedConsumable;

        //TODO: revisit this code. it is probably better to do a square cast shape and
        //... sort collisions based on distance to the center of the cast, then
        //... pick the best object based on that

        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, radius: 0.1f, out hit, maxDistance: 1f, layerMask: consumableLayers) ||
            Physics.SphereCast(ray, radius: 0.2f, out hit, maxDistance: 2f, layerMask: dashLayers))
        {
            GameObject hitObject = hit.collider.gameObject;

            // if looking at the same object, no changes needed
            if (hitObject == previousViewedConsumable) { return; }

            var consumableData = hitObject.GetComponent<Consumable>();
            var canConsumeTarget = consumableData != null && consumableData.isConsumable;
            viewedConsumable = canConsumeTarget ? consumableData : null;
        }
        else
        {
            viewedConsumable = null;
        }

        if (previousViewedConsumable != null)
        {
            previousViewedConsumable.outline.enabled = false;
        }
        if (viewedConsumable != null)
        {
            viewedConsumable.outline.enabled = true;
        }
    }



    private void GameInput_Eat()
    {
        if (inventory.isFull) { return; }
        if (viewedConsumable == null) { return; }

        var viewedObject = viewedConsumable.gameObject;
        ConsumeObject(viewedObject);

        var distance = (transform.position - viewedObject.transform.position).magnitude;
        if (distance >= 1f)
        {
            player.SetTarget(viewedObject.transform.position);
            player.SetState(PlayerState.eating);
        }
    }

    private void GameInput_Puke()
    {
        if (inventory.isEmpty) { return; }

        Consumable itemToPlace = inventory.PopItem();
        if (itemToPlace == null) { return; } // case when you press puke and eat at the same time
        var pukeDir = transform.forward;
        var targetPosition = transform.position + pukeDir * pukeDistance;
        itemToPlace.PlaceAt(targetPosition);
    }

    /// <summary>
    /// Consumes the object and updates the inventory
    /// </summary>
    private void ConsumeObject(GameObject obj)
    {
        var consumableData = obj.GetComponent<Consumable>();
        if (consumableData == null || !consumableData.isConsumable) { return; }

        inventory.PushItem(consumableData);
        consumableData.SetState(ItemState.beingConsumed);
    }

    private void UpdateMass()
    {
        var totalItemMass = inventory.totalMass;

        //TODO: make this update the mass, doesnt have to be 1-1 or even linear
        int currInventoryCount = inventory.itemCount;

        // doing it this way would essentially add on the average of the inventory's mass to the player, prob not what we want...
        //rb.mass = baseMass + (totalItemMass / currInventoryCount);

        // just scale the mass by some factor multiplied by the number of items in the inventory, once we tweak the masses of the objects, we can care about factoring that into this calc
        rb.mass = baseMass + currInventoryCount * MASSFACTOR;
        gameObject.transform.localScale = baseScale + new Vector3(0.2f, 0.2f, 0.2f) * currInventoryCount;
    }


}
