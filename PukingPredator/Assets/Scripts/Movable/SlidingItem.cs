using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingItem : MovableItem
{
    [SerializeField]
    private List<Vector3> path;

    [SerializeField]
    private float speed = 2f;

    private int nextPoint = 0;
    private int direction = 1;

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
