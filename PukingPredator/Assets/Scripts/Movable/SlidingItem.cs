using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingItem : MovableItem
{
    [SerializeField]
    private List<Vector3> path;

    [SerializeField]
    private float speed = 2f;

    /// <summary>
    /// Used to check if the player is on the platofrm and moves the player if grounded.
    /// </summary>
    [SerializeField]
    private CollisionTracker collisionTracker;

    private int nextPoint = 0;
    private int direction = 1;
    private Player player;
    bool updatePlayer = false;

    protected override void Start()
    {
        base.Start();
        collisionTracker.SubscribeOnCollision(OnTrackerEnter);
        collisionTracker.SubscribeOnLeave(OnTrackerExit);
    }


    private void OnTrackerEnter()
    {
        StartCoroutine(CheckForCollision());
    }

    private void OnTrackerExit()
    {
        updatePlayer = false;
    }

    private IEnumerator CheckForCollision()
    {
        player = collisionTracker.collisions[0].GetComponent<Player>();

        while (collisionTracker.collisions.Count != 0)
        {
            if (player.movement.isGrounded)
            {
                updatePlayer = true;
                break;
            }

            yield return null;
        }
    }

    private void Update()
    {
        if (path == null || path.Count < 2)
            return;

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        Vector3 targetPosition = path[nextPoint];
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (updatePlayer)
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            nextPoint += direction;

            if (nextPoint >= path.Count || nextPoint < 0)
            {
                direction *= -1; // Reverse
                nextPoint += direction * 2;
            }
        }
    }
}
