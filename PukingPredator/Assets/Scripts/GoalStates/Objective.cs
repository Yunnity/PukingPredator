using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{

    private ObjectiveTracker tracker;    

    // Register this objective with the objective tracker
    public void registerObjectiveTracker(ObjectiveTracker ot)
    {
        tracker = ot;
        tracker.addCollectionObject();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            tracker.removeCollectionObject();
            Destroy(this.gameObject);
        }
    }
}
