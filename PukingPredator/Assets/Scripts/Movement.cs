using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : InputBehaviour
{
    /// <summary>
    /// The max time to hold the jump button for.
    /// </summary>
    [SerializeField]
    private float buttonTime = 0.4f;

    /// <summary>
    /// If the instance can currently jump.
    /// </summary>
    private bool canJump => !isCoyoteFinished;

    /// <summary>
    /// Radius of the sphere used for collision checks.
    /// </summary>
    private float groundCheckRadius = 0.2f;

    /// <summary>
    /// Layers of ground objects.
    /// </summary>
    [SerializeField]
    private LayerMask groundLayer;

    /// <summary>
    /// If the instance is currently on the ground.
    /// </summary>
    private bool isGrounded;

    /// <summary>
    /// If the instance has been ungrounded for more than a certain amount of time
    /// </summary>
    private bool isCoyoteFinished;

    /// <summary>
    /// Timer for coyote time
    /// </summary>
    private float coyoteTime = 0f;

    /// <summary>
    /// Amount of leniency for coyote time
    /// </summary>
    private float coyoteTimeLimit = 0.2f;

    /// <summary>
    /// If the instance is currently in a jump.
    /// </summary>
    private bool isJumping;

    /// <summary>
    /// The force applied when jumping.
    /// </summary>
    [SerializeField]
    private float jumpForce;

    /// <summary>
    /// How long a jump has been going on for.
    /// </summary>
    private float jumpTime;

    /// <summary>
    /// Can be used to disable the movement code so velocity is not reset.
    /// </summary>
    public bool isManualMovementEnabled = true;

    /// <summary>
    /// The direction of the players recent movement input with the camera rotation applied.
    /// </summary>
    private Vector3 moveDir;

    /// <summary>
    /// The speed that the player moves in any given direction.
    /// </summary>
    private float moveSpeed = 10f;

    /// <summary>
    /// A reference to the camera, used for correcting movement direction.
    /// </summary>
    [SerializeField]
    private GameObject playerCamera;

    /// <summary>
    /// The rigidbody of the instance.
    /// </summary>
    private Rigidbody rb;

    private float baseMass;

    private const float MOVESPEEDFACTOR = 4f;

    private PlayerAnimation playerAnimation;



    private void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        rb = GetComponent<Rigidbody>();
        baseMass = rb.mass;

        if (playerCamera == null) { playerCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0]; }

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
            rb.velocity = moveDir * moveSpeed / (1 + Mathf.Exp(rb.mass - baseMass) * MOVESPEEDFACTOR) + new Vector3(0, rb.velocity.y, 0);
        }
    }

    private void Update()
    {
        if (gameInput == null) { return; }

        //walking code
        Vector2 inputVector = gameInput.movementInput;
        moveDir =   isManualMovementEnabled
                ?   Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0) * new Vector3(inputVector.x, 0, inputVector.y)
                :   rb.velocity.HorizontalProjection().normalized;

        float turnSpeed = 10f;
        if (inputVector.magnitude > 0)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * turnSpeed);
        }
        Vector3 position = transform.position;
        //jumping code
        isGrounded = Physics.CheckSphere(new Vector3(position.x, position.y + 0.1f, position.z), groundCheckRadius, groundLayer);

        if (isJumping)
        {
            jumpTime += Time.deltaTime;

            if (jumpTime > buttonTime) { isJumping = false; }
        }
        else if (!isJumping) { jumpTime = 0; }

        if (isGrounded)
        {
            if (jumpTime == 0)
            {
                isJumping = false;
                isCoyoteFinished = false;
                coyoteTime = 0f;
            }
        }
        else if (!isGrounded && !isCoyoteFinished)
        {
            if (coyoteTime > coyoteTimeLimit) { isCoyoteFinished = true; }
            else { coyoteTime += Time.deltaTime; }
        }
    }

    public void GameInput_JumpDown()
    {
        if (!canJump) { return; }

        isCoyoteFinished = true;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isJumping = true;
        jumpTime = 0;

        playerAnimation?.StartJumpAnim();
    }
}
