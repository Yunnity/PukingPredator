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
    /// Default size of the character.
    /// </summary>
    private Vector3 baseScale;

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
    /// Multiplier for the mass of items in the players inventory. 0.05 means 5%
    /// of the weight of items is added to the players mass when consumed.
    /// </summary>
    private const float MASS_FACTOR = 0.05f;

    /// <summary>
    /// The player component.
    /// </summary>
    private Player player;

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
    private const float MIN_PUKE_FORCE = 1f;
    private const float MAX_PUKE_FORCE = 20f;
    private const float MAX_PUKE_DURATION = 2f;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// The object that the player is looking at to eat.
    /// </summary>
    private Consumable viewedConsumable = null;

    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();

        baseMass = rb.mass;

        inventory.onChange += UpdateMass;
        
        baseScale = gameObject.transform.localScale;

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

        player.EatObject(viewedObject);
    }
    
    private void GameInput_Puke()
    {
        if (inventory.isEmpty) { return; }

        Consumable itemToPuke = inventory.PopItem();
        itemToPuke.SetState(ItemState.beingPuked);

        //puke forward and with a little force upwards
        var pukeDir = transform.forward + Vector3.up * 0.1f;

        Rigidbody itemRb = itemToPuke.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            //TODO: make this code work if there is no rigid body. perhaps just set
            //...the velocity and if there was a rigid body then we can reduce the velocity
            //...while calculating it based on the mass?
            itemRb.AddForce(pukeDir * pukeForce * itemRb.mass, ForceMode.Impulse);
        }
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
        int currInventoryCount = inventory.itemCount;

        //TODO: change this to use totalItemMass instead of the count once masses are fine tuned
        rb.mass = baseMass + currInventoryCount * MASS_FACTOR;
        gameObject.transform.localScale = baseScale + new Vector3(0.2f, 0.2f, 0.2f) * currInventoryCount;
    }

}
