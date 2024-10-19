using UnityEngine;

public class Collectable : MonoBehaviour
{
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
