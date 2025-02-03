using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class Aim : InputBehaviour
{
    /// <summary>
    /// The player component.
    /// </summary>
    private Player player;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// The radius of spherecasts when the players scale is 1.
    /// </summary>
    private float baseRadius = 0.2f;

    /// <summary>
    /// The max distance when the players scale is 1.
    /// </summary>
    private float baseRange = 2f;

    private bool isAiming = false;
    public float radius = 1f;
    private LineRenderer lineRenderer;
    private int segments = 32;

    private LineRenderer startSphereRenderer;
    private LineRenderer endSphereRenderer;
    private LineRenderer pathRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();

        startSphereRenderer = CreateLineRenderer(Color.green);
        endSphereRenderer = CreateLineRenderer(Color.red);
        pathRenderer = CreateLineRenderer(Color.cyan, false);

        Subscribe(InputEvent.onAim, () => isAiming = true);
        Subscribe(InputEvent.onEat, () => isAiming = false);
    }

    private void Update()
    {
        if (isAiming)
        {
            DrawWireSphereCast();
            player.SetState(PlayerState.aiming);
        }
        else
        {
            ClearWireSphereCast();
            player.SetState(PlayerState.standing);
        }
    }


    void ClearWireSphereCast()
    {
        startSphereRenderer.enabled = false;
        endSphereRenderer.enabled = false;
        pathRenderer.enabled = false;
    }

    void DrawWireSphereCast()
    {
        startSphereRenderer.enabled = true;
        endSphereRenderer.enabled = true;
        pathRenderer.enabled = true;
        float multiplier = transform.localScale.y;
        float startBehindOffset = 0.1f * multiplier;
        float maxDistance = baseRange * multiplier + startBehindOffset;
        float radius = baseRadius * multiplier;

        Vector3 startPosition = transform.position + (radius + 0.05f) * Vector3.up - transform.forward * startBehindOffset;
        Vector3 endPosition = startPosition + transform.forward.normalized * maxDistance;

        // Draw wire spheres at start and end positions
        DrawWireSphere(startSphereRenderer, startPosition, radius);
        DrawWireSphere(endSphereRenderer, endPosition, radius);

        // Draw line path
        pathRenderer.positionCount = 2;
        pathRenderer.SetPositions(new Vector3[] { startPosition, endPosition });
    }

    LineRenderer CreateLineRenderer(Color color, bool loop = true)
    {
        GameObject obj = new GameObject("WireSphere");
        obj.transform.SetParent(transform);
        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.positionCount = loop ? segments + 1 : 2;
        lr.loop = loop;
        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;
        lr.useWorldSpace = true;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        return lr;
    }

    void DrawWireSphere(LineRenderer lr, Vector3 position, float radius)
    {
        Vector3[] points = new Vector3[segments + 1];
        float angleStep = 360f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            points[i] = position + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }

        lr.positionCount = points.Length;
        lr.SetPositions(points);
    }

}