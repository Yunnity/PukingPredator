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
public class Consumable : Interactable
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
    public const float consumptionCutoff = 0.085f;

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
    /// If the origin has been centered already or not.
    /// </summary>
    private bool hasOriginBeenCentered = false;

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
    public Inventory inventory
    {
        get => _inventory;
        set
        {
            _inventory = value;
            if (_inventory == null) { return; }

            inventoryNumber = _inventory.itemCount - 1;
            inventoryTimeOffset = Time.time;
            inventoryTimeScale = UnityEngine.Random.Range(1.2f, 1.6f);

            inventoryTargetAngle = UnityEngine.Random.Range(0, Mathf.PI * 2);

            if (_inventory.itemCount == 1) { return; }
            //Make sure the angle isnt too close to the previous item if there is one.
            var prevItem = _inventory.PeekItem(1);
            while (Mathf.Abs(prevItem.inventoryTargetAngle - inventoryTargetAngle) < Mathf.PI/3)
            {
                inventoryTargetAngle = UnityEngine.Random.Range(0, Mathf.PI * 2);
            }
        }
    }
    private Inventory _inventory;

    /// <summary>
    /// How far from the bottom of the stack the item is. If it was the first item
    /// eaten, the value will be 0.
    /// </summary>
    private int inventoryNumber;

    /// <summary>
    /// Angle relative to the players forward direction (in radians).
    /// </summary>
    public float inventoryTargetAngle { get; private set; }
    
    /// <summary>
    /// The position that this object should be in if its in an inventory.
    /// </summary>
    private Vector3 inventoryTargetPosition
    {
        get
        {
            var targetPosition = inventory.transform.position;

            var scale = 0.05f * inventory.transform.lossyScale.y;

            var player = inventory.owner;
            var playerForward = player.transform.forward;
            var offsetDirection = playerForward.RotateAboutY(inventoryTargetAngle).normalized;
            targetPosition += offsetDirection * scale;

            //shift vertically based on position in inventory
            //[0, N-1] -> [-1, 1]
            var verticalMultiplier = (2f * inventoryNumber / (inventory.maxCount - 1)) - 1f;
            targetPosition.y += verticalMultiplier * scale;

            //apply random motion up and down
            targetPosition.y += 0.02f * Mathf.Sin(inventoryTimeScale * (Time.time - inventoryTimeOffset));

            return targetPosition;
        }
    }

    private float inventoryTimeOffset;
    private float inventoryTimeScale;

    /// <summary>
    /// Can be used to lock an item.
    /// </summary>
    public bool isConsumable = true;

    /// <summary>
    /// If the component is currently decaying.
    /// </summary>
    public bool isDecaying => decayTimer != null;

    public override bool isInteractable => isConsumable;

    /// <summary>
    /// The mass of the instance.
    /// </summary>
    public float mass => rb != null ? rb.mass : 1;

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
    public const float pukeRate = 12f;

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

    [SerializeField]
    public Sprite thumbnail;

    protected override void Awake()
    {
        base.Awake();

        initialLayer = gameObject.layer;
        initialScale = gameObject.transform.localScale;
        rb = GetComponent<Rigidbody>();

        //Setup the state events
        foreach (ItemState itemState in Enum.GetValues(typeof(ItemState)))
        {
            stateEvents.Add(itemState, new State());
        }

        stateEvents[ItemState.inWorld].onEnter += ResetLayer;
        stateEvents[ItemState.inWorld].onEnter += ResetScale;
        stateEvents[ItemState.inWorld].onExit += SetLayerToConsumed;
        stateEvents[ItemState.inWorld].onExit += CenterOrigin;

        stateEvents[ItemState.beingConsumed].onEnter += EnablePhysics;

        stateEvents[ItemState.beingConsumed].onUpdate += UpdateBeingConsumed;

        stateEvents[ItemState.inInventory].onEnter += ClampShrunkScale;
        stateEvents[ItemState.inInventory].onEnter += StartDecay;
        stateEvents[ItemState.inInventory].onEnter += SetGravityDisabled;
        stateEvents[ItemState.inInventory].onExit += SetGravityEnabled;
        stateEvents[ItemState.inInventory].onUpdate += FollowInventory;

        if (rb != null) { stateEvents[ItemState.beingPuked].onEnter += ResetVelocity; }
        stateEvents[ItemState.beingPuked].onEnter += SetLayerToBeingPuked;
        stateEvents[ItemState.beingPuked].onUpdate += UpdateBeingPuked;
    }

    public void Update()
    {
        stateEvents[state].onUpdate?.Invoke();
    }



    private void CenterOrigin()
    {
        if (hasOriginBeenCentered) { return; }
        hasOriginBeenCentered = true;

        gameObject.CenterOrigin();
    }

    private void ClampShrunkScale()
    {
        gameObject.transform.localScale = initialScale * consumptionCutoff;
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

    private void EnablePhysics()
    {
        var physicsObject = GetComponent<PhysicsBehaviour>();
        if (physicsObject != null)
        {
            physicsObject.EnablePhysics();
        }
        else if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    private void FollowInventory()
    {
        //TODO: make rotation gradual?
        gameObject.transform.position = inventoryTargetPosition;
        //gameObject.transform.position = Vector3.Lerp(
        //    gameObject.transform.position,
        //    inventoryTargetPosition,
        //    10f * Time.deltaTime * Mathf.Max(1f, 2*Vector3.Distance(transform.position, inventory.transform.position))
        //);
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

    private void SetLayerToBeingPuked()
    {
        SetLayer(GameLayer.beingPuked);
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
        var currRate = consumptionRate * Time.deltaTime;
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, inventoryTargetPosition, currRate);
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

}
