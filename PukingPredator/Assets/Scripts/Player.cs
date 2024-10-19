using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    eating,
    standing
}

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private Dash dash;

    /// <summary>
    /// The layers that interactable objects can be on.
    /// </summary>
    [SerializeField]
    private LayerMask interactableLayers;

    [SerializeField]
    private Inventory _inventory;
    public Inventory inventory => _inventory;

    private Movement movement;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;

    public PlayerState state { get; private set; } = PlayerState.standing;
    public Dictionary<PlayerState, State> stateEvents = new();

    /// <summary>
    /// The object that the player is looking at.
    /// </summary>
    public Interactable viewedInteractable { get; private set; } = null;



    private void Awake()
    {
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
        {
            stateEvents.Add(state, new State());
        }

        rb = GetComponent<Rigidbody>();

        dash = GetComponent<Dash>();
        dash.onComplete += DashFinished;

        movement = GetComponent<Movement>();
        AudioManager.Instance.PlayBackground();
    }

    private void Update()
    {
        stateEvents[state].onUpdate?.Invoke();
        UpdateViewed();
    }



    private void DashFinished()
    {
        SetState(PlayerState.standing);
        movement.isManualMovementEnabled = true;
    }

    /// <summary>
    /// Makes the player dash towards the target.
    /// </summary>
    /// <param name="obj"></param>
    public void EatObject(GameObject obj)
    {
        var deltaPosition = obj.transform.position - transform.position;
        var distance = deltaPosition.magnitude;

        SetState(PlayerState.eating);
        movement.isManualMovementEnabled = false;
        dash.DashTo(obj);
    }

    public void SetState(PlayerState newState)
    {
        var previousState = state;
        if (newState == previousState) { return; }
        state = newState;
    }

    /// <summary>
    /// Updates the object currently being viewed by the player and updates
    /// outlines accordingly.
    /// </summary>
    private void UpdateViewed()
    {
        var previousViewedInteractable = viewedInteractable;

        //TODO: revisit this code. it is probably better to do a square cast shape and
        //... sort collisions based on distance to the center of the cast, then
        //... pick the best object based on that

        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (inventory.isFull)
        {
            viewedInteractable = null;
        }
        else if (Physics.SphereCast(ray, radius: 0.2f, out hit, maxDistance: 2f, layerMask: interactableLayers))
        {
            GameObject hitObject = hit.collider.gameObject;

            // if looking at the same object, no changes needed
            if (hitObject == previousViewedInteractable) { return; }

            var interactableData = hitObject.GetComponent<Interactable>();
            var canInteract = interactableData != null && interactableData.isInteractable;
            viewedInteractable = canInteract ? interactableData : null;
        }
        else
        {
            viewedInteractable = null;
        }

        if (previousViewedInteractable != null)
        {
            previousViewedInteractable.outline.enabled = false;
        }
        if (viewedInteractable != null)
        {
            viewedInteractable.outline.enabled = true;
        }
    }
}