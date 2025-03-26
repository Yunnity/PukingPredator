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
    private Timer pauseTimer;
    private bool isWaiting = false;

    protected override void Start()
    {
        base.Start();
        collisionTracker.SubscribeOnCollision(OnTrackerEnter);
        collisionTracker.SubscribeOnLeave(OnTrackerExit);

        pauseTimer = gameObject.AddComponent<Timer>();
        pauseTimer.onTimerComplete += () => isWaiting = false;

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
        if (path == null || path.Count < 2 || isWaiting)
            return;

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        Vector3 targetPosition = path[nextPoint];

        Vector3 starting = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (updatePlayer)
        {
            Vector3 offset = transform.position - starting;
            player.transform.position += offset;

        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            nextPoint += direction;

            if (nextPoint >= path.Count || nextPoint < 0)
            {
                direction *= -1; // Reverse
                nextPoint += direction * 2;

                // Waits once it reaches the end of its path for 1 second
                isWaiting = true;
                pauseTimer.StartTimer(1.0f);
            }
        }
    }
}
