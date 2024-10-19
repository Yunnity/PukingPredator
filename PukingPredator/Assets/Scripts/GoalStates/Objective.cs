using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{

    private ObjectiveTracker tracker;
    private Consumable consumable;
    private bool isCollected = false; // to prevent bugs from repeated calls

    private void Start()
    {
        consumable = GetComponent<Consumable>();
        consumable.stateEvents[ItemState.inInventory].onEnter += RemoveCollectionObject;
        consumable.stateEvents[ItemState.inInventory].onExit += AddCollectionObject;
    }

    // Register this objective with the objective tracker
    public void registerObjectiveTracker(ObjectiveTracker ot)
    {
        tracker = ot;
        tracker.addCollectionObject();
    }

    private void RemoveCollectionObject()
    {
        if (!isCollected)
        {
            tracker.removeCollectionObject();
            isCollected = true;
        }
    }

    private void AddCollectionObject()
    {
        if ( isCollected )
        {
            tracker.addCollectionObject();
            isCollected = false;
        }
    }
}
