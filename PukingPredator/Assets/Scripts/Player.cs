using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Inventory inventory;
    [SerializeField] private LayerMask consumable;
    [SerializeField] private GameObject playerCamera;

    public float groundCheckRadius = 0.5f; // Radius of the sphere
    public LayerMask groundLayer; // Layer of ground objects
    private Rigidbody rigidBody;
    private float baseMoveSpeed = 10f;
    private float moveSpeed;
    private float baseJumpForce = 7f;
    private float jumpForce;

    private Vector3 moveDir;

    [SerializeField]
    private bool isGrounded;
    private Coroutine ungroundedCoroutine = null;
    private bool isJumpOnCooldown = false;
    private float jumpCooldown = 0.25f;
    private float coyotteTime = 0.15f;
    private bool canJump
    {
        get => isGrounded && !isJumpOnCooldown;
    }

    private float pukeDistance = 2f;

    private void Start()
    {
        gameInput.onEatAction += GameInput_OnEat;
        gameInput.onPukeAction += GameInput_OnPuke;
        gameInput.onResetLevelAction += ResetLevel;

        inventory.onChange += UpdateMovementAttributes;
        UpdateMovementAttributes();

        rigidBody = GetComponent<Rigidbody>();
        playerCamera = playerCamera == null ? GameObject.FindGameObjectsWithTag("MainCamera")[0] : playerCamera;
    }

    private void GameInput_OnPuke(object sender, System.EventArgs e)
    {
        Item itemToPlace = inventory.PopItem();

        if (itemToPlace is null) { return; }

        var pukeDir = transform.forward;
        itemToPlace.PlaceAt(transform.position + pukeDir*pukeDistance);
    }

    private void GameInput_OnEat(object sender, System.EventArgs e)
    {
        if (inventory.isFull) { return; }

        // Define the ray, starting from the player's position, shooting forward
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        float sphereRadius = 0.5f; // Adjust as needed

        // Perform the sphere cast
        if (Physics.SphereCast(ray, sphereRadius, out hit, 5f, consumable)) // 5f is the maximum ray distance
        {
            // Get the GameObject that was hit
            GameObject hitObject = hit.collider.gameObject;
            GameObject newItemObject = new GameObject("NewItem");  // Create a new empty GameObject
            Item newItem = newItemObject.AddComponent<Item>();     // Add the Item component

            // Initialize the item with properties from the hit object and push it to the inventory
            newItem.Initialize(hitObject.gameObject, gameObject);
            inventory.PushItem(newItem);
        }
    }

    public void ResetLevel(object sender, System.EventArgs e)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = gameInput.GetInputVectorNormalized();
        moveDir = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0) * new Vector3(inputVector.x, 0, inputVector.y);

        float turnSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * turnSpeed);

        GroundedUpdate();
        if (canJump && Input.GetButton("Jump"))
        {
            Debug.Log("jumping");
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            StartCoroutine(ApplyJumpCooldown());
        }
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = moveDir * moveSpeed + new Vector3( 0, rigidBody.velocity.y, 0);
    }

    private void GroundedUpdate()
    {
        var onGround = Physics.CheckSphere(transform.position + Vector3.down * 0.75f, groundCheckRadius, groundLayer);
        if (onGround)
        {
            isGrounded = true;
            if (ungroundedCoroutine is not null)
            {
                StopCoroutine(ungroundedCoroutine);
                ungroundedCoroutine = null;
            }
        }
        else if (isGrounded && ungroundedCoroutine is null)
        {
            ungroundedCoroutine = StartCoroutine(SetGroundedFalse(coyotteTime));
        }

    }
    private IEnumerator ApplyJumpCooldown()
    {
        isJumpOnCooldown = true;
        yield return new WaitForSeconds(jumpCooldown);
        isJumpOnCooldown = false;
    }

    private IEnumerator SetGroundedFalse(float delay)
    {
        yield return new WaitForSeconds(delay);
        isGrounded = false;
        ungroundedCoroutine = null;
    }

    private void UpdateMovementAttributes()
    {
        //TODO: make values dynamic (would be more intuitive); alter mass and drag instead? (makes a difference for collisions and falling)

        var totalItemMass = inventory.totalMass;

        moveSpeed = baseMoveSpeed - totalItemMass;
        jumpForce = baseJumpForce - totalItemMass;
    }

}