using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PukeTrajectory : InputBehaviour
{
    /// <summary>
    /// Layers that objects being puked can collide with.
    /// TODO: this can be changed to not have to use a manually declared layer
    /// mask by copying what the following video does at about 8:30
    /// https://www.youtube.com/watch?v=8mGZBYsSXcQ
    /// </summary>
    [SerializeField]
    private LayerMask layerMask;

    private LineRenderer lineRenderer;

    /// <summary>
    /// Maximum number of points to render in the arc.
    /// </summary>
    private int maxPoints = 100;

    /// <summary>
    /// The puking component from the player.
    /// </summary>
    [SerializeField]
    private Puking puking;

    /// <summary>
    /// Time between each point in seconds.
    /// </summary>
    private float timeStep = 0.05f;

    /// <summary>
    /// Offset relative to the original position. Below this height, the arc
    /// will be cut off.
    /// </summary>
    [SerializeField]
    private float yCutoff = 0f;



    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        Subscribe(InputEvent.onPukeStart, ActivateVisuals);
        Subscribe(InputEvent.onPuke, DeactivateVisuals);
        Subscribe(InputEvent.onPukeCancel, DeactivateVisuals);
    }

    private void Update()
    {
        if (!lineRenderer.enabled) { return; }

        DrawTrajectory();
    }



    private void ActivateVisuals()
    {
        if (puking.inventory.isEmpty) { return; }
        lineRenderer.enabled = true;
    }
    private void DeactivateVisuals()
    {
        lineRenderer.enabled = false;
    }

    private void DrawTrajectory()
    {
        if (gameInput.pukeHoldDuration < GameInput.minHoldTime * 2)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = maxPoints;
        Vector3[] points = new Vector3[maxPoints];

        Vector3 currentPosition = transform.position;
        float initialHeight = currentPosition.y;
        Vector3 velocity = puking.pukeVelocity;

        int pointIndex = 0;
        for (int i = 0; i < maxPoints; i++)
        {
            // Store the current point in the points array
            points[pointIndex++] = currentPosition;

            // Break if object falls below the yThreshold on the way down
            if (currentPosition.y < initialHeight + yCutoff && velocity.y < 0)
            {
                lineRenderer.positionCount = pointIndex; // Limit the line to points plotted so far
                break;
            }

            // Check if there is an obstacle ahead
            if (Physics.Raycast(currentPosition, velocity.normalized, out RaycastHit hit, velocity.magnitude * timeStep, layerMask))
            {
                // If a collision is detected, end the trajectory at the collision point
                points[pointIndex++] = hit.point;
                lineRenderer.positionCount = pointIndex;
                break;
            }

            // Update position and velocity for the next point based on physics
            currentPosition += velocity * timeStep;
            velocity += Physics.gravity * timeStep; // Apply gravity to the velocity
        }

        // Assign the calculated points to the LineRenderer to display the trajectory
        lineRenderer.SetPositions(points);
    }
}