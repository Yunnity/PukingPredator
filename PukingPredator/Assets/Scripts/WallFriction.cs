using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WallFriction : MonoBehaviour
{
    [SerializeField]
    private PhysicMaterial airMaterial;

    private CollisionTracker collisionTracker;

    [SerializeField]
    private Player player;

    private Collider playerCollider;



    private void Start()
    {
        playerCollider = player.GetComponent<Collider>();
        collisionTracker = GetComponent<CollisionTracker>();
    }

    private void Update()
    {
        if (collisionTracker.collisions.Count > 0)
        {
            player.movement.isSlidingOnWall = true;
            playerCollider.material = airMaterial;
        }
        else
        {
            player.movement.isSlidingOnWall = false;
        }
    }
}
