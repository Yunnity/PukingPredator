using System.Collections.Generic;
using UnityEngine;

public enum ItemSize
{
    small,
    medium,
    large
}

public enum ItemState
{
    beingConsumed,
    inInventory,
    beingPuked,
}

public class Item : MonoBehaviour
{
    /// <summary>
    /// The lerp factor used when shrinking items.
    /// </summary>
    [SerializeField]
    private float consumptionRate = 0.25f;

    /// <summary>
    /// The relative scale at which an object will be treated as consumed.
    /// ie 0.05 means 5% of original size.
    /// </summary>
    private float consumptionCutoff = 0.05f;

    /// <summary>
    /// The initial scale of the instance before being eaten. Used to restore
    /// its size to normal after being puked.
    /// </summary>
    private Vector3 initialScale;

    /// <summary>
    /// The game object for this item in the inventory. It may be inactive or
    /// active.
    /// </summary>
    private GameObject instance;

    /// <summary>
    /// Reference to the inventory this item belongs to
    /// </summary>
    public Inventory inventory;

    /// <summary>
    /// The mass of an object. This is based on its size.
    /// </summary>
    public float mass => sizeToMass[size];

    /// <summary>
    /// The entity that is holding the item.
    /// </summary>
    private GameObject owner;

    /// <summary>
    /// Size of the item, affects mass of the player
    /// </summary>
    /// TODO: vary sizes
    public ItemSize size { get; private set; } = ItemSize.small;

    /// <summary>
    /// The mappings from sizes to masses
    /// </summary>
    private Dictionary<ItemSize, float> sizeToMass = new()
    {
        {ItemSize.small, 1f},
        {ItemSize.medium, 1.5f},
        {ItemSize.large, 2f},
    };

    /// <summary>
    /// If the item is entering, leaving, or sitting in the inventory.
    /// </summary>
    public ItemState state { get; private set; } = ItemState.beingConsumed;

    Timer timer;
    public void Update()
    {
        // Handles the case if you start consuming as soon as an item is destroyed
        if (instance == null)
        {
            if (gameObject) Destroy(gameObject);
            return;
        }

        switch (state)
        {
            case ItemState.inInventory:
                return;

            case ItemState.beingConsumed:
                var ownerPosition = owner.transform.position;
                instance.transform.position = Vector3.Lerp(instance.transform.position, ownerPosition, consumptionRate);
                instance.transform.localScale = Vector3.Lerp(instance.transform.localScale, Vector3.zero, consumptionRate);

                var hasBeenConsumed = instance.transform.localScale.magnitude / initialScale.magnitude < consumptionCutoff;
                if (hasBeenConsumed)
                {
                    timer.StartTimer();
                    instance.SetActive(false);
                    state = ItemState.inInventory;
                }

                break;

            case ItemState.beingPuked:
                //TODO: implement gradual puking
                break;
        }

    }



    /// <summary>
    /// Set up the inventory item.
    /// </summary>
    /// <param name="itemInstance"></param>
    /// <param name="itemOwner"></param>
    public void Initialize(GameObject itemInstance, GameObject itemOwner)
    {
        instance = itemInstance;
        owner = itemOwner;

        // Store the scale so it can later be used to return the object to the
        // right size
        initialScale = instance.transform.localScale;
        
        timer = gameObject.AddComponent<Timer>();
        timer.Init(5f);
        timer.OnTimerComplete += StartDecay;
    }

    /// <summary>
    /// Moves the item to the position and reactivates it.
    /// </summary>
    /// <param name="position"></param>
    public void PlaceAt(Vector3 position)
    {
        if (state != ItemState.inInventory) { return; }
        if (instance == null)
        {
            if (gameObject) Destroy(gameObject);
            return;
        }

        instance.transform.localScale = initialScale;
        //TODO: we should probably undo this at some point OR make it so you have control over the rotation
        instance.transform.rotation = Quaternion.identity;

        //reset the velocity 
        var rigidBody = instance.GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }

        instance.transform.position = position;
        instance.SetActive(true);
    }

    /// <summary>
    /// Replaces the current item with the decayed item or destroys it if there
    /// is no such item
    /// </summary>
    public void StartDecay(object sender, System.EventArgs e)
    {
        if (instance == null) { return; }
        ItemReplace replacement = instance.GetComponent<ItemReplace>();
        Item nextItem = null;

        // Checks if theres an item that will replace the current one after the decay
        if (replacement != null)
        {
            GameObject nextInstance = replacement.nextPrefab;

            // Creates new game object and item for the object that will replace the current
            GameObject replaceItemObject = new GameObject("ReplaceItem");
            nextItem = replaceItemObject.AddComponent<Item>();

            GameObject replaceObject = Instantiate(nextInstance, transform.position, transform.rotation);
            replaceObject.SetActive(false);

            nextItem.Initialize(replaceObject, owner);
            nextItem.inventory = inventory;
        }

        // Tries replacing the item in the inventory with the new decayed item
        if (!inventory.ReplaceItem(this, nextItem))
        {
            // This means the item did not decay in the player it decayed outside
            if (replacement)
            {
                replacement.Replace();
            }
            else
            {
                Destroy(instance);
            };
        };
    }
}