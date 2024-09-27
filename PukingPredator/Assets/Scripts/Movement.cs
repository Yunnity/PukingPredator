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
    private bool canJump => isGrounded && !isJumping;

    /// <summary>
    /// Radius of the sphere used for collision checks.
    /// </summary>
    private float groundCheckRadius = 0.45f;

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
    /// If a jump was cancelled early by releasing the jump button.
    /// </summary>
    private bool isJumpCancelled;

    /// <summary>
    /// If the instance is currently in a jump.
    /// </summary>
    private bool isJumping;

    /// <summary>
    /// Force applied downwards to reduce jump height if you let go early.
    /// </summary>
    [SerializeField]
    private float jumpCancelRate = 25;

    /// <summary>
    /// The force applied when jumping.
    /// </summary>
    [SerializeField]
    public float jumpForce = 6;

    /// <summary>
    /// How long a jump has been going on for.
    /// </summary>
    private float jumpTime;

    /// <summary>
    /// The direction of the players recent movement input with the camera rotation applied.
    /// </summary>
    private Vector3 moveDir;

    /// <summary>
    /// The speed that the player moves in any given direction.
    /// </summary>
    private float moveSpeed = 10;

    /// <summary>
    /// A reference to the camera, used for correcting movement direction.
    /// </summary>
    [SerializeField]
    private GameObject playerCamera;

    /// <summary>
    /// The rigidbody of the instance.
    /// </summary>
    private Rigidbody rb;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (playerCamera == null) { playerCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0]; }

        Subscribe(EventType.onJumpDown, GameInput_JumpDown);
        Subscribe(EventType.onJumpUp, GameInput_JumpUp);
    }

    private void FixedUpdate()
    {
        //walking code
        //TODO: make this depend on the mass of the object
        rb.velocity = moveDir * moveSpeed + new Vector3(0, rb.velocity.y, 0);

        //jumping code
        //Reduce jump height if the button is released early
        if (isJumpCancelled && isJumping && rb.velocity.y > 0)
        {
            rb.AddForce(Vector3.down * jumpCancelRate);
            if (rb.velocity.y <= 0) { isJumping = false; }
        }
    }

    private void Update()
    {
        if (gameInput == null) { return; }

        //walking code
        Vector2 inputVector = gameInput.movementInput;
        moveDir = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0) * new Vector3(inputVector.x, 0, inputVector.y);

        float turnSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * turnSpeed);

        //jumping code
        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);

        if (isJumping)
        {
            jumpTime += Time.deltaTime;

            if (jumpTime > buttonTime) { isJumping = false; }
        }
    }



    public void GameInput_JumpDown()
    {
        if (!canJump) { return; }

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isJumping = true;
        isJumpCancelled = false;
        jumpTime = 0;
    }

    public void GameInput_JumpUp()
    {
        if (isJumping) { isJumpCancelled = true; }
    }
}
