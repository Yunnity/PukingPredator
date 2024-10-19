using System;
using UnityEngine;

/// <summary>
/// This dash script is based on the functions in the following graph
/// for the height curve, and it uses a fixed horizontal velocity.
/// https://www.desmos.com/calculator/mu4bgefbi2
/// </summary>
[RequireComponent(typeof(Player))]
public class Dash : InputBehaviour
{
    /// <summary>
    /// The vector for the horizontal direction of the dash.
    /// </summary>
    private Vector3 direction;

    /// <summary>
    /// Set based on the distance to the target at the start of the dash.
    /// </summary>
    private float duration;
    private float timeElapsed;

    /// <summary>
    /// The height of the dash based. It is based on the original distance.
    /// </summary>
    private float height => 1.5f * Mathf.Log10(duration + 1);

    /// <summary>
    /// How much the height should change by the end of the jump.
    /// </summary>
    private float heightChange;
    private const float MAX_HEIGHT_CHANGE = 0.35f;

    /// <summary>
    /// Horizontal speed when dashing
    /// </summary>
    private float speed = 3.5f;

    /// <summary>
    /// If a dash is currently happening.
    /// </summary>
    private bool isDashing = false;

    public Action onComplete = null;

    /// <summary>
    /// The player component.
    /// </summary>
    private Player player;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// Based on the following graphs:
    /// https://www.desmos.com/calculator/mu4bgefbi2
    /// </summary>
    private float verticalVelocity
    {
        get => (-2 * timeElapsed + duration) * 4 * height / Mathf.Pow(duration, 2) + timeElapsed / duration;
    }



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();

        Subscribe(InputEvent.onEat, GameInput_Eat);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing) { return; }

        timeElapsed += Time.deltaTime;

        if (timeElapsed >= duration)
        {
            OnComplete();
            return;
        }

        var vel = direction * speed;
        vel.y = verticalVelocity;
        rb.velocity = vel;
    }



    private void GameInput_Eat()
    {
        var viewedInteractable = player.viewedInteractable;

        if (viewedInteractable == null) { return; }

        var viewedObject = viewedInteractable.gameObject;
        DashTo(viewedObject);
    }



    public void DashTo(GameObject target)
    {
        isDashing = true;
        timeElapsed = 0;

        player.SetState(PlayerState.dashing);
        player.movement.isManualMovementEnabled = false;

        var targetPosition = target.transform.position;
        var deltaPosition = targetPosition - transform.position;

        heightChange = Mathf.Clamp(deltaPosition.y, 0, MAX_HEIGHT_CHANGE);
        deltaPosition.y = 0;

        direction = deltaPosition.normalized;
        duration = deltaPosition.magnitude / speed;
    }

    private void OnComplete()
    {
        isDashing = false;

        player.SetState(PlayerState.standing);
        player.movement.isManualMovementEnabled = true;

        var vel = rb.velocity;
        vel.y = 0;
        rb.velocity = vel;

        onComplete?.Invoke();
    }
}