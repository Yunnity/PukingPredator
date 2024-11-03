using UnityEngine;

public class Collectable : Interactable
{
    public override bool isInteractable => true;

    public CollectableTracker tracker;
    public GameObject particleEffect;

    public int id;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(GameTag.player))
        {
            tracker.EmitParticles(collision.transform.position);
            tracker.CollectOne();
            CheckpointManager.Instance.AddCollectable(id);
            Destroy(gameObject);
        }
    }
}
