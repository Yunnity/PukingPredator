using UnityEngine;

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
    private float consumptionRate = 0.025f;

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
    /// The entity that is holding the item.
    /// </summary>
    private GameObject owner;

    /// <summary>
    /// If the item is entering, leaving, or sitting in the inventory.
    /// </summary>
    public ItemState state = ItemState.beingConsumed;



    public void Update()
    {
        switch(state)
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

    }

    /// <summary>
    /// Moves the item to the position and reactivates it.
    /// </summary>
    /// <param name="position"></param>
    public void PlaceAt(Vector3 position)
    {
        if (state != ItemState.inInventory) { return; }

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

}
