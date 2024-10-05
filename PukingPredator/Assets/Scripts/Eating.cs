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

    // Time when the right mouse button was pressed
    private float rightMouseButtonDownTime;
    private float pukeForce;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        baseMass = rb.mass;

        rightMouseButtonDownTime = 0f;

        inventory.onChange += UpdateMass;

        Subscribe(InputEvent.onEat, GameInput_Eat);
        Subscribe(InputEvent.onPuke, GameInput_Puke);
    }


    private void Update()
    {
        if (Input.GetKey("mouse 1"))
        {
            rightMouseButtonDownTime += Time.deltaTime;
        }
        else
        {
            rightMouseButtonDownTime = 0f;
        }
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

            rightMouseButtonDownTime = 0f;
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
            float itemMass = itemRb.mass;
            if (rightMouseButtonDownTime > 1f)
            {
                pukeForce = Mathf.Clamp(rightMouseButtonDownTime * 10, 10f, 50f);
                Debug.Log($"pukeForce = {pukeForce}");
                Debug.Log($"pukeForce * itemMass = {pukeForce * itemMass}");
                itemRb.AddForce(pukeDir * pukeForce * itemMass, ForceMode.Impulse);
                //rb.AddForce(-pukeDir * pukeForce * 20f, ForceMode.Impulse);
            }
        }
        rightMouseButtonDownTime = 0f;
    }

    private void UpdateMass()
    {
        var totalItemMass = inventory.totalMass;

        //TODO: make this update the mass, doesnt have to be 1-1 or even linear
        rb.mass = baseMass;
    }
}
