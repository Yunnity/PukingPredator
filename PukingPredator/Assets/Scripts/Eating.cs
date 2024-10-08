using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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
    /// The players inventory.
    /// </summary>
    [SerializeField]
    private Inventory inventory;

    /// <summary>
    /// The distance that items spawn ahead of the player when puking. (TEMP)
    /// </summary>
    private float pukeDistance = 2f;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        baseMass = rb.mass;

        inventory.onChange += UpdateMass;

        Subscribe(InputEvent.onEat, GameInput_Eat);
        Subscribe(InputEvent.onPuke, GameInput_Puke);
    }



    private void GameInput_Eat()
    {
        //if (inventory.isFull) { return; }

        // Define the ray, starting from the player's position, shooting forward
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, radius: 0.2f, out hit, maxDistance: 1f, layerMask: consumableLayers))
        {
            // Get the GameObject that was hit
            GameObject hitObject = hit.collider.gameObject;
            Process_Consumption(hitObject);
        }
        else if (Physics.SphereCast(ray, radius: 0.2f, out hit, maxDistance: 5f, layerMask: consumableLayers))
        {
            // Dashes if the object is far
            GameObject hitObject = hit.collider.gameObject;
            Process_Consumption(hitObject);

            Player player = player = this.GetComponent<Player>();
            if (player != null)
            {
                player.SetTarget(hitObject.transform.position);
                player.SetState(MovementState.eating);
            }
        }
    }

    private void GameInput_Puke()
    {
        if (inventory.isEmpty) { return; }

        Consumable itemToPlace = inventory.PopItem();

        var pukeDir = transform.forward;
        var targetPosition = transform.position + pukeDir * pukeDistance;
        itemToPlace.PlaceAt(targetPosition);
    }

    /// <summary>
    /// Consumes the object and updates the inventory
    /// </summary>
    private void Process_Consumption(GameObject hitObject)
    {
        var consumableData = hitObject.GetComponent<Consumable>();
        if (consumableData == null || !consumableData.isConsumable) { return; }

        inventory.PushItem(consumableData);
        consumableData.SetState(ItemState.beingConsumed);
    }

    private void UpdateMass()
    {
        var totalItemMass = inventory.totalMass;

        //TODO: make this update the mass, doesnt have to be 1-1 or even linear
        rb.mass = baseMass;
    }


}
