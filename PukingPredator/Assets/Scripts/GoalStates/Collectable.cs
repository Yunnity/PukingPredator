using UnityEngine;

public class Collectable : Interactable
{
    public override bool isInteractable => true;

    public CollectableTracker tracker;



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            tracker.CollectedOne();
            Destroy(gameObject);
        }
    }
}
