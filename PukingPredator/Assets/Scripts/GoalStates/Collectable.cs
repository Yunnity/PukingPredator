using UnityEngine;

public class Collectable : Interactable
{
    public override bool isInteractable => true;

    public CollectableTracker tracker;
    public GameObject particleEffect;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            tracker.emitParticles(collision.transform.position);
            tracker.CollectedOne();
            Destroy(gameObject);
        }
    }
}
