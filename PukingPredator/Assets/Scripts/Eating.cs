using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Eating : InputBehaviour
{
    /// <summary>
    /// The mass of the player before having eaten anything.
    /// </summary>
    private float baseMass;

    /// <summary>
    /// Default size of the character.
    /// </summary>
    private Vector3 baseScale;

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
    /// Multiplier for the mass of items in the players inventory. 0.05 means 5%
    /// of the weight of items is added to the players mass when consumed.
    /// </summary>
    private const float MASS_FACTOR = 0.05f;

    /// <summary>
    /// The distance that items spawn ahead of the player when puking. (TEMP)
    /// </summary>
    private float pukeDistance = 2f;

    /// <summary>
    /// Force applied to object when puked. Depends on how long the puke button
    /// was held down for.
    /// </summary>
    private float pukeForce
    {
        get {
            float holdPercent = Mathf.Clamp(gameInput.pukeHoldDuration / MAX_PUKE_DURATION, 0, 1);
            return Mathf.Lerp(MIN_PUKE_FORCE, MAX_PUKE_FORCE, holdPercent);
        }
    }
    private const float MIN_PUKE_FORCE = 10f;
    private const float MAX_PUKE_FORCE = 50f;
    private const float MAX_PUKE_DURATION = 2f;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;

    

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        baseMass = rb.mass;

        inventory.onChange += UpdateMass;
        
        baseScale = gameObject.transform.localScale;

        Subscribe(InputEvent.onEat, GameInput_Eat);
        Subscribe(InputEvent.onPuke, GameInput_Puke);
    }



    private void GameInput_Eat()
    {
        if (inventory.isFull) { return; }

        // Define the ray, starting from the player's position, shooting forward
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, radius: 0.5f, out hit, maxDistance: 5f, layerMask: consumableLayers))
        {
            // Get the GameObject that was hit
            GameObject hitObject = hit.collider.gameObject;

            var consumableData = hitObject.GetComponent<Consumable>();
            if (consumableData == null || !consumableData.isConsumable)
            {
                return;
            }

            inventory.PushItem(consumableData);
            consumableData.SetState(ItemState.beingConsumed);
        }
    }

    private void GameInput_Puke()
    {
        if (inventory.isEmpty) { return; }

        Consumable itemToPlace = inventory.PopItem();

        var pukeDir = transform.forward;
        var targetPosition = transform.position + pukeDir * pukeDistance;
        itemToPlace.PlaceAt(targetPosition);

        Rigidbody itemRb = itemToPlace.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            //TODO: make this code work if there is no rigid body. perhaps just set
            //...the velocity and if there was a rigid body then we can reduce the velocity
            //...while calculating it based on the mass?
            itemRb.AddForce(pukeDir * pukeForce * itemRb.mass, ForceMode.Impulse);
        }
    }

    private void UpdateMass()
    {
        var totalItemMass = inventory.totalMass;
        int currInventoryCount = inventory.itemCount;

        //TODO: change this to use totalItemMass instead of the count once masses are fine tuned
        rb.mass = baseMass + currInventoryCount * MASS_FACTOR;
        gameObject.transform.localScale = baseScale + new Vector3(0.2f, 0.2f, 0.2f) * currInventoryCount;
    }
}
