using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : InputBehaviour
{
    private float baseMass;

    /// <summary>
    /// If the instance can currently jump.
    /// </summary>
    private bool canJump => !isJumping && Time.time <= lastTimeGrounded + coyoteTime;

    /// <summary>
    /// Amount of leniency for coyote time.
    /// </summary>
    private float coyoteTime = 0.2f;

    [SerializeField]
    private CollisionTracker groundCollisionTracker;

    /// <summary>
    /// If the instance is currently on the ground.
    /// </summary>
    private bool isGrounded = false;

    /// <summary>
    /// If the instance is currently in a jump.
    /// </summary>
    private bool isJumping;

    /// <summary>
    /// Can be used to disable the movement code so velocity is not reset.
    /// </summary>
    public bool isManualMovementEnabled = true;

    /// <summary>
    /// How long you can press the jump button before being able to jump
    /// and still have it go off.
    /// </summary>
    private float jumpBufferTime = 0.2f;

    private Coroutine jumpCoroutine;

    /// <summary>
    /// The force applied when jumping.
    /// </summary>
    [SerializeField]
    private float jumpForce;

    private float lastTimeGrounded = float.MinValue;

    /// <summary>
    /// The direction of the players recent movement input with the camera rotation applied.
    /// </summary>
    private Vector3 moveDir;

    /// <summary>
    /// The speed that the player moves in any given direction.
    /// </summary>
    private float moveSpeed = 10f;

    /// <summary>
    /// Used to change move speed with size.
    /// </summary>
    private const float MOVE_SPEED_FACTOR = 4f;

    private PlayerAnimation playerAnimation;

    /// <summary>
    /// A reference to the camera, used for correcting movement direction.
    /// </summary>
    [SerializeField]
    private GameObject playerCamera;

    /// <summary>
    /// The rigidbody of the instance.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// How quickly the player model turns.
    /// </summary>
    private float turnSpeed = 10f;



    private void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        rb = GetComponent<Rigidbody>();
        baseMass = rb.mass;

        if (playerCamera == null) { playerCamera = GameObject.FindGameObjectsWithTag(GameTag.mainCamera)[0]; }

        Subscribe(InputEvent.onJumpDown, GameInput_JumpDown);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, playerCamera.transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private void FixedUpdate()
    {
        //walking code
        // moveSpeed - (rb.mass - baseMass) alters the movespeed such that we just subtract a constant (0.1f) from the movespeed for each item in the inventory
        // kind of weird since jumping is tied directly to mass since we use forces, but horizontal movement is not
        if (isManualMovementEnabled)
        {
            rb.velocity = moveDir * moveSpeed / (1 + Mathf.Exp(rb.mass - baseMass) * MOVE_SPEED_FACTOR) + new Vector3(0, rb.velocity.y, 0);
        }
    }

    private void Update()
    {
        if (gameInput == null) { return; }

        #region walking code
        Vector2 inputVector = gameInput.movementInput;
        moveDir =   isManualMovementEnabled
                ?   Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0) * new Vector3(inputVector.x, 0, inputVector.y)
                :   rb.velocity.HorizontalProjection().normalized;

        if (inputVector.magnitude > 0)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * turnSpeed);
        }

        if (isGrounded && (rb.velocity.x != 0 || rb.velocity.z != 0))
        {
            AudioManager.Instance.PlaySFX(AudioManager.ClipName.Walking, true);
        }
        #endregion

        #region jumping code
        isGrounded =    groundCollisionTracker.collisions.Count > 0
                    &&  !isJumping;
        //disable gravity when on the ground to prevent sliding down stairs
        rb.useGravity = !isGrounded || rb.velocity.y > 0;

        if (isGrounded) { lastTimeGrounded = Time.time; }
        if (isJumping) { isJumping = rb.velocity.y > 0; }
        #endregion
    }



    public void GameInput_JumpDown()
    {
        if (jumpCoroutine != null) { StopCoroutine(jumpCoroutine); }
        jumpCoroutine = StartCoroutine(TryJump());
    }

    /// <summary>
    /// Gets the GameInput instance and then activates all subscriptions.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TryJump()
    {
        var pressTime = Time.time;
        while (Time.time <= pressTime + jumpBufferTime)
        {
            if (canJump)
            {
                Jump();
                jumpCoroutine = null;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
        jumpCoroutine = null;
    }

    private void Jump()
    {
        if (!canJump) { return; }
        isJumping = true;

        //artifically decreasing the time to protect against double jumps
        lastTimeGrounded = float.MinValue;

        //overwrite the vertical velocity based on the jump force
        var vel = rb.velocity;
        vel.y = jumpForce / rb.mass;
        rb.velocity = vel;

        playerAnimation?.StartJumpAnim();
        AudioManager.Instance.PlaySFX(AudioManager.ClipName.Jump, true);
    }
}
