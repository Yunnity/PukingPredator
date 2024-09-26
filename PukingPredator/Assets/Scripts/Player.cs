using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    /// <summary>
    /// The mass of the player before having eaten anything.
    /// </summary>
    private float baseMass;

    /// <summary>
    /// The base move speed before added weight is applied. (TEMP)
    /// </summary>
    private float baseMoveSpeed = 10f;

    /// <summary>
    /// The layers that consumable objects can be on. (TEMP???)
    /// </summary>
    [SerializeField]
    private LayerMask consumableLayers;

    /// <summary>
    /// The handler for user input.
    /// </summary>
    private GameInput gameInput;

    /// <summary>
    /// The players inventory.
    /// </summary>
    [SerializeField]
    private Inventory inventory;

    /// <summary>
    /// The direction of the players recent movement input with the camera rotation applied.
    /// </summary>
    private Vector3 moveDir;

    /// <summary>
    /// The speed that the player moves in any given direction.
    /// </summary>
    private float moveSpeed;

    /// <summary>
    /// A reference to the camera, used for correcting movement direction.
    /// </summary>
    [SerializeField]
    private GameObject playerCamera;

    /// <summary>
    /// The distance that items spawn ahead of the player when puking. (TEMP)
    /// </summary>
    private float pukeDistance = 2f;

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        baseMass = rb.mass;
    }

    private void FixedUpdate()
    {
        rb.velocity = moveDir * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    private void Start()
    {
        inventory.onChange += UpdateMovementAttributes;
        UpdateMovementAttributes();

        if (playerCamera == null) { playerCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0]; }
    }

    void Update()
    {
        if (!TryGetGameInput()) { return; }

        Vector2 inputVector = gameInput.movementInput;
        moveDir = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0) * new Vector3(inputVector.x, 0, inputVector.y);

        float turnSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * turnSpeed);
    }



    private void GameInput_Eat()
    {
        if (inventory.isFull) { return; }

        // Define the ray, starting from the player's position, shooting forward
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, radius: 0.5f, out hit, maxDistance: 5f, layerMask: consumableLayers))
        {
            // Get the GameObject that was hit
            GameObject hitObject = hit.collider.gameObject;

            var consumableData = hitObject.GetComponent<Consumable>();
            if (consumableData == null || !consumableData.isConsumable) { return; }

            inventory.PushItem(consumableData);
            consumableData.SetState(ItemState.beingConsumed);
        }
    }

    private void GameInput_Puke()
    {
        if (inventory.isEmpty) { return; }

        Consumable itemToPlace = inventory.PopItem();

        var pukeDir = transform.forward;
        itemToPlace.PlaceAt(transform.position + pukeDir * pukeDistance);
    }

    public void GameInput_ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Gets the GameInput instance if it hasnt already been fetched.
    /// </summary>
    /// <returns>If gameInput has been successfully loaded by the end of the function.</returns>
    private bool TryGetGameInput()
    {
        if (gameInput != null) { return true; }

        gameInput = GameInput.Instance;
        if (gameInput == null) { return false; }

        gameInput.onEat += GameInput_Eat;
        gameInput.onPuke += GameInput_Puke;
        gameInput.onResetLevel += GameInput_ResetLevel;

        return true;
    }

    private void UpdateMovementAttributes()
    {
        //TODO: make values dynamic (would be more intuitive); alter mass and drag instead? (makes a difference for collisions and falling)

        var totalItemMass = inventory.totalMass;

        moveSpeed = baseMoveSpeed - totalItemMass;
    }

}