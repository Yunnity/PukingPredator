using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Jump : MonoBehaviour
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
    /// Force applied downwards to reduce jump height if you let go early.
    /// </summary>
    [SerializeField]
    private float cancelRate = 25;

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
    /// The force applied when jumping.
    /// </summary>
    [SerializeField]
    public float jumpForce = 6;

    /// <summary>
    /// How long a jump has been going on for.
    /// </summary>
    private float jumpTime;

    /// <summary>
    /// The rigidbody of the instance.
    /// </summary>
    private Rigidbody rb;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            isJumping = true;
            isJumpCancelled = false;
            jumpTime = 0;
        }

        if (isJumping)
        {
            jumpTime += Time.deltaTime;
            if (Input.GetKeyUp(KeyCode.Space)) { isJumpCancelled = true; }

            if (jumpTime > buttonTime) { isJumping = false; }
        }
    }

    private void FixedUpdate()
    {
        //Reduce jump height if the button is released early
        if (isJumpCancelled && isJumping && rb.velocity.y > 0)
        {
            rb.AddForce(Vector3.down * cancelRate);
            if (rb.velocity.y <= 0) { isJumping = false; }
        }
    }

}
