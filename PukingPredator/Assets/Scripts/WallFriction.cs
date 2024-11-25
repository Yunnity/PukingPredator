using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WallFriction : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private PhysicMaterial airMaterial;

    private Collider playerCollider;
    private Collider frictionCollider;

    private CollisionTracker collisiontracker;

    private void Start()
    {
        playerCollider = player.GetComponent<Collider>();
        frictionCollider = GetComponent<Collider>();
        collisiontracker = GetComponent<CollisionTracker>();
    }

    private void Update()
    {
        if (collisiontracker.collisions.Count > 0)
        {
            player.movement.SetWallSlide(true);
            playerCollider.material = airMaterial;
        }
        else
        {
            player.movement.SetWallSlide(false);
        }
    }
}
