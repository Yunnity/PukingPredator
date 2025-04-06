using UnityEngine;

public class ProjectileBlast : MonoBehaviour
{
    /// <summary>
    /// The hitbox for what objects will be activated.
    /// </summary>
    [SerializeField]
    private CollisionTracker blastCollisionTracker;

    private const float blastForce = 2f;

    [HideInInspector]
	public GameObject owner;

    /// <summary>
    /// Used to remove the collision detector after the blast is done.
    /// </summary>
	private CollisionDetector ownerCollisionDetector;



    void Start()
	{
        ownerCollisionDetector = owner.AddComponent<CollisionDetector>();
        ownerCollisionDetector.Subscribe(TryExplode);

    }

    void OnDestroy()
    {
		Destroy(ownerCollisionDetector);
    }



    private void Explode()
    {
        foreach (var collision in blastCollisionTracker.collisions)
        {
            Rigidbody hitRB = collision.GetComponent<Rigidbody>();
            if (hitRB == null) { continue; }

            PhysicsBehaviour pb = collision.GetComponent<PhysicsBehaviour>();
            if (pb != null)
            {
                pb.EnablePhysics();
            }

            hitRB.AddExplosionForce(blastForce, transform.position, 5f, 0.01f, ForceMode.VelocityChange);
        }
    }
    private void TryExplode()
    {
        var rb = owner.GetComponent<Rigidbody>();
        if (rb.velocity.HorizontalProjection().magnitude > 1.5f) { Explode(); }

        //destroy self, only do one blast collision
        Destroy(gameObject);
    }
}

