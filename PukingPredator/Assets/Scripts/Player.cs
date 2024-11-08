using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    dashing,
    standing
}

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    /// <summary>
    /// The mass of the player before having eaten anything.
    /// </summary>
    private float baseMass;

    /// <summary>
    /// Default size of the character.
    /// </summary>
    private Vector3 baseScale;

    private InteractablePicker interactablePicker;

    [SerializeField]
    private Inventory _inventory;
    public Inventory inventory => _inventory;

    /// <summary>
    /// Multiplier for the mass of items in the players inventory. 0.05 means 5%
    /// of the weight of items is added to the players mass when consumed.
    /// </summary>
    private const float MASS_FACTOR = 0.05f;

    public Movement movement { get; private set; }

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// The current scale relative to the base scale.
    /// </summary>
    public float relativeScale => transform.localScale.magnitude / baseScale.magnitude;

    /// <summary>
    /// How much the inventory size impacts the size of the player
    /// </summary>
    private const float SCALE_FACTOR = 0.2f;

    /// <summary>
    /// How quickly the players size grows.
    /// </summary>
    private const float SCALE_RATE = 8f;

    public PlayerState state { get; private set; } = PlayerState.standing;
    public Dictionary<PlayerState, State> stateEvents = new();

    /// <summary>
    /// The object that the player is looking at.
    /// </summary>
    public Interactable targetInteractable => interactablePicker.targetInteractable;

    /// <summary>
    /// Used for gradual scale changes.
    /// </summary>
    private Vector3 targetScale;



    private void Awake()
    {
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            stateEvents.Add(state, new State());
        }

        rb = GetComponent<Rigidbody>();
        baseMass = rb.mass;
        baseScale = gameObject.transform.localScale;
        targetScale = baseScale;

        movement = GetComponent<Movement>();

        interactablePicker = GetComponent<InteractablePicker>();

        inventory.onChange += UpdateMassAndSize;
    }

    private void Update()
    {
        stateEvents[state].onUpdate?.Invoke();

        var rate = SCALE_RATE * Time.deltaTime;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, rate);
    }



    public void SetState(PlayerState newState)
    {
        var previousState = state;
        if (newState == previousState) { return; }
        state = newState;
    }

    private void UpdateMassAndSize()
    {
        var totalItemMass = inventory.totalMass;
        int currInventoryCount = inventory.itemCount;

        //TODO: change this to use totalItemMass instead of the count once masses are fine tuned
        rb.mass = baseMass + currInventoryCount * MASS_FACTOR;
        targetScale = baseScale * (1 + SCALE_FACTOR * currInventoryCount);
    }
}