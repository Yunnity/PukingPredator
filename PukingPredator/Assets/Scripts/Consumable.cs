using UnityEngine;

public enum ItemState
{
    inWorld,
    beingConsumed,
    inInventory,
    beingPuked,
}
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Consumable : MonoBehaviour
{
    [SerializeField]
    private bool _canDecay = false;
    /// <summary>
    /// If the instance can decay.
    /// </summary>
    public bool canDecay => _canDecay;

    /// <summary>
    /// The lerp factor used when shrinking items.
    /// </summary>
    private float consumptionRate = 0.2f;

    /// <summary>
    /// The relative scale at which an object will be treated as consumed.
    /// ie 0.05 means 5% of original size.
    /// </summary>
    [SerializeField]
    private float consumptionCutoff = 0.05f;

    /// <summary>
    /// The prefab the instance should become when it decays.
    /// </summary>
    [SerializeField]
    private GameObject decayInto;

    /// <summary>
    /// The time it takes to decay.
    /// </summary>
    private float decayTime = 5f;

    /// <summary>
    /// The timer used to track the decay.
    /// </summary>
    public Timer decayTimer { get; private set; }

    /// <summary>
    /// Collider attached to the instance.
    /// </summary>
    private Collider hitbox;

    /// <summary>
    /// The initial scale of the instance before being eaten. Used to restore
    /// its size to normal after being puked.
    /// </summary>
    public Vector3 initialScale { get; private set; }

    /// <summary>
    /// Reference to the inventory this item belongs to.
    /// </summary>
    [HideInInspector]
    public Inventory inventory;

    /// <summary>
    /// Can be used to lock an item.
    /// </summary>
    public bool isConsumable = true;

    /// <summary>
    /// If the component is currently decaying.
    /// </summary>
    public bool isDecaying => decayTimer != null;

    /// <summary>
    /// The mass of the instance.
    /// </summary>
    //TODO: make this use the mass of the game object (swap "1f" to "rb.mass")
    public float mass => 1f;

    /// <summary>
    /// Rigid body attached to the instance.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// If the item is entering, leaving, or sitting in the inventory.
    /// </summary>
    public ItemState state { get; private set; } = ItemState.inWorld;



    private void Awake()
    {
        initialScale = gameObject.transform.localScale;
        rb = GetComponent<Rigidbody>();
        hitbox = GetComponent<Collider>();
    }

    public void Update()
    {
        if (state == ItemState.inWorld) { return; }

        var ownerPosition = inventory.owner.transform.position;

        switch (state)
        {
            case ItemState.beingConsumed:
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, ownerPosition, consumptionRate);
                gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, Vector3.zero, consumptionRate);

                var hasBeenConsumed = gameObject.transform.localScale.magnitude / initialScale.magnitude < consumptionCutoff;
                if (hasBeenConsumed) { SetState(ItemState.inInventory); }

                break;

            case ItemState.inInventory:
                //TODO: add some periodic + random offset so objects float around in you?
                gameObject.transform.position = ownerPosition;

                break;

            case ItemState.beingPuked:
                //TODO: implement gradual puking
                break;
        }

    }



    private void Decay()
    {
        if (decayInto == null)
        {
            if (inventory != null) { inventory.RemoveItem(this); }
        }
        else
        {
            GameObject replaceObject = Instantiate(decayInto, transform.position, transform.rotation);

            //Copy the state of this object over
            var replaceConsumableData = replaceObject.GetComponent<Consumable>();
            replaceConsumableData.SetState(state);
            //TODO: the following line doesnt work but it needs to be implemented in case an item is converted while being spit out or consumed
            //replaceObject.transform.localScale *= gameObject.transform.localScale.magnitude / initialScale.magnitude;

            if (inventory != null) { inventory.ReplaceItem(this, replaceConsumableData); }
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Moves the item to the position and reactivates it.
    /// </summary>
    /// <param name="position"></param>
    public void PlaceAt(Vector3 position)
    {
        if (state != ItemState.inInventory) { return; }

        gameObject.transform.localScale = initialScale;

        var previousAngles = gameObject.transform.eulerAngles;
        gameObject.transform.eulerAngles = new Vector3(0, previousAngles.y, 0);

        gameObject.transform.position = position;
        SetState(ItemState.inWorld);
    }

    /// <summary>
    /// Swap between states
    /// </summary>
    /// <param name="state"></param>
    public void SetState(ItemState state)
    {
        if (state == this.state) { return; }

        switch (state)
        {
            case ItemState.inWorld:
                rb.isKinematic = false;
                hitbox.enabled = true;

                //reset the velocity 
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                break;

            case ItemState.inInventory:
                rb.isKinematic = true;
                hitbox.enabled = false;

                //TODO: swap this to be based on the initial scale, not just [1,1,1]
                gameObject.transform.localScale = new Vector3(1f, 1f, 1f) * consumptionCutoff;

                StartDecay();
                break;
        }
        this.state = state;
    }

    /// <summary>
    /// Can be called to make the instance start to decay.
    /// </summary>
    public void StartDecay()
    {
        if (!canDecay || isDecaying) { return; }

        //if there is already a timer, it must be decaying already.
        decayTimer = gameObject.AddComponent<Timer>();
        decayTimer.StartTimer(decayTime);
        decayTimer.onTimerComplete += Decay;
    }
}
