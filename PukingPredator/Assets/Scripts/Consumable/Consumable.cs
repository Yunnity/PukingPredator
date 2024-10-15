using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState
{
    inWorld,
    beingConsumed,
    inInventory,
    beingPuked,
}
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
    private float consumptionRate = 7f;

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
    /// The layer that the object started out on.
    /// </summary>
    private int initialLayer;

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
    public float mass => rb != null ? rb.mass : 1;

    /// <summary>
    /// The color of the outline when close to the player.
    /// </summary>
    private Color outlineColor = Color.white;

    /// <summary>
    /// The range at which objects start/stop showing an outline.
    /// </summary>
    private float outlineDetectionRadius = 5f;

    /// <summary>
    /// Size of the outline visual.
    /// </summary>
    private const float OUTLINE_RADIUS = 2.2f;

    /// <summary>
    /// Settings for the outline of the consumable (toggled on via enable when player is near)
    /// </summary>
    public Outline outline;

    /// <summary>
    /// The game object that owns the consumable (ie the player).
    /// </summary>
    public GameObject owner => inventory.owner;

    /// <summary>
    /// The transform of the owner.
    /// </summary>
    public Transform ownerTransform => inventory.owner.transform;

    /// <summary>
    /// The lerp factor used when growing items.
    /// </summary>
    private float pukeRate = 12f;

    /// <summary>
    /// Rigid body attached to the instance.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// If the item is entering, leaving, or sitting in the inventory.
    /// </summary>
    public ItemState state { get; private set; } = ItemState.inWorld;

    /// <summary>
    /// The actions that get executed when entering a state.
    /// </summary>
    public Dictionary<ItemState, State> stateEvents = new();



    protected virtual void Awake()
    {
        initialLayer = gameObject.layer;
        initialScale = gameObject.transform.localScale;
        rb = GetComponent<Rigidbody>();
        hitbox = GetComponent<Collider>();

        ConfigureOutline();

        //Setup the state events
        foreach (ItemState itemState in Enum.GetValues(typeof(ItemState)))
        {
            stateEvents.Add(itemState, new State());
        }

        stateEvents[ItemState.inWorld].onEnter += ResetLayer;
        stateEvents[ItemState.inWorld].onEnter += SetGravityEnabled;
        stateEvents[ItemState.inWorld].onEnter += ResetScale;
        //stateEvents[ItemState.inWorld].onUpdate += UpdateProximityOutline;
        stateEvents[ItemState.inWorld].onExit += SetLayerToConsumed;
        stateEvents[ItemState.inWorld].onExit += SetGravityDisabled; 
        stateEvents[ItemState.inWorld].onExit += DisableKinematic;

        stateEvents[ItemState.beingConsumed].onUpdate += UpdateBeingConsumed;

        //stateEvents[ItemState.inInventory].onEnter += DisablePhysics;
        stateEvents[ItemState.inInventory].onEnter += ClampShrunkScale;
        stateEvents[ItemState.inInventory].onEnter += StartDecay;
        //stateEvents[ItemState.inInventory].onExit += EnablePhysics;
        stateEvents[ItemState.inInventory].onUpdate += FollowOwner;

        if (rb != null) { stateEvents[ItemState.beingPuked].onEnter += ResetVelocity; }
        stateEvents[ItemState.beingPuked].onUpdate += UpdateBeingPuked;
    }

    public void Update()
    {
        stateEvents[state].onUpdate?.Invoke();
    }



    private void ClampShrunkScale()
    {
        gameObject.transform.localScale = initialScale * consumptionCutoff;
    }

    private void ConfigureOutline()
    {
        outline = gameObject.GetComponent<Outline>();
        if (outline == null) { outline = gameObject.AddComponent<Outline>(); }

        outline.OutlineWidth = OUTLINE_RADIUS;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = outlineColor;
        outline.enabled = false;
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

    private void DisablePhysics()
    {
        rb.isKinematic = true;
        hitbox.enabled = false;
    }

    private void EnablePhysics()
    {
        rb.isKinematic = false;
        hitbox.enabled = true;
    }

    private void FollowOwner()
    {
        //TODO: add some periodic + random offset so objects float around in you?
        gameObject.transform.position = ownerTransform.position;
    }

    #region Gravity
    private void SetGravity(bool enabled)
    {
        foreach (GameObject target in gameObject.GetDescendantsAndSelf())
        {
            var targetRB = target.GetComponent<Rigidbody>();
            
            if (targetRB == null) { continue; }
            targetRB.useGravity = enabled;
        }
    }

    private void SetGravityDisabled()
    {
        SetGravity(false);
    }

    private void SetGravityEnabled()
    {
        SetGravity(true);
    }
    #endregion

    #region Kinematic
    public void SetRBKinematic(bool isKinematic)
    {
        rb.isKinematic = isKinematic;
    }

    public void DisableKinematic()
    {
        SetRBKinematic(false);
    }
    #endregion

    /// <summary>
    /// Moves the item to the position and reactivates it.
    /// </summary>
    /// <param name="position"></param>
    public void PlaceAt(Vector3 position)
    {
        if (state != ItemState.inInventory) { return; }

        var previousAngles = gameObject.transform.eulerAngles;
        gameObject.transform.eulerAngles = new Vector3(0, previousAngles.y, 0);

        gameObject.transform.position = position;
        SetState(ItemState.inWorld);
    }

    private void ResetScale()
    {
        gameObject.transform.localScale = initialScale;
    }

    private void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    #region Set Layer
    /// <summary>
    /// Set the layer of the instance and all of its children to the matching layer name.
    /// </summary>
    /// <param name="layer"></param>
    private void SetLayer(int layer)
    {
        foreach (GameObject target in gameObject.GetDescendantsAndSelf())
        {
            target.layer = layer;
        }
    }

    private void ResetLayer()
    {
        SetLayer(initialLayer);
    }

    private void SetLayerToConsumed()
    {
        SetLayer(GameLayer.consumed);
    }
    #endregion

    /// <summary>
    /// Swap between states
    /// </summary>
    /// <param name="state"></param>
    public void SetState(ItemState newState)
    {
        var previousState = state;
        if (newState == previousState) { return; }
        state = newState;

        stateEvents[previousState].onExit?.Invoke();
        stateEvents[newState].onEnter?.Invoke();
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

    private void UpdateBeingConsumed()
    {
        var ownerPosition = ownerTransform.position;
        var currRate = consumptionRate * Time.deltaTime;
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, ownerPosition, currRate);
        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, Vector3.zero, currRate);

        var hasBeenConsumed = gameObject.transform.localScale.magnitude / initialScale.magnitude < consumptionCutoff;
        if (hasBeenConsumed) { SetState(ItemState.inInventory); }
    }

    private void UpdateBeingPuked()
    {
        var currRate = pukeRate * Time.deltaTime;
        //lerp towards a number slightly bigger than the original scale
        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, initialScale * 1.05f, currRate);

        var hasFinishedPuking = gameObject.transform.localScale.magnitude >= initialScale.magnitude;
        if (hasFinishedPuking) { SetState(ItemState.inWorld); }
    }

    private void UpdateProximityOutline()
    {
        if (outline == null) { return; }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, outlineDetectionRadius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.CompareTag(GameTag.player))
            {
                outline.enabled = true;
                return;
            }
        }
        outline.enabled = false;
        return;
    }

}
