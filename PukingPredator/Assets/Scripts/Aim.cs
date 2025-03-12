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
    /// The rigidbody of the player.
    /// </summary>
    public GameObject aimVisual;

    /// <summary>
    /// The radius of aimVisual when the players scale is 1.
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

    private LineRenderer pathRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();

        pathRenderer = CreateLineRenderer(Color.green, false);

        Subscribe(InputEvent.onAim, () => isAiming = true);
        Subscribe(InputEvent.onEat, () => isAiming = false);
    }

    private void Update()
    {
        if (isAiming)
        {
            if (gameInput.eatHoldDuration < GameInput.minHoldTime * 2) { return; }
            aimVisual.SetActive(true);
            DrawAimVisual();
            player.SetState(PlayerState.aiming);
        }
        else
        {
            aimVisual.SetActive(false);
            player.SetState(PlayerState.standing);
            Clear();
        }
    }


    void Clear()
    {
        pathRenderer.enabled = false;
    }

    void DrawAimVisual()
    {
        pathRenderer.enabled = true;
        var multiplier = transform.localScale.y;
        var startBehindOffset = 0.1f * multiplier;
        var maxDistance = baseRange * multiplier + startBehindOffset;
        
        var radius = baseRadius * multiplier;

        Vector3 startPosition = transform.position + (radius + 0.05f) * Vector3.up - transform.forward * startBehindOffset;
        Vector3 endPosition = startPosition + transform.forward.normalized * maxDistance;

        aimVisual.transform.localScale = new Vector3(maxDistance, aimVisual.transform.localScale.y, maxDistance);

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

}