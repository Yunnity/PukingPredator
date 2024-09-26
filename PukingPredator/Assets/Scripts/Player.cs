using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Inventory inventory;
    [SerializeField] private LayerMask consumable;
    [SerializeField] private GameObject playerCamera;

    private Rigidbody rb;
    private float baseMoveSpeed = 10f;
    private float moveSpeed;

    private Vector3 moveDir;

    private float pukeDistance = 2f;

    private void Start()
    {
        gameInput.onEat += GameInput_OnEat;
        gameInput.onPuke += GameInput_OnPuke;
        gameInput.onResetLevel += GameInput_ResetLevel;

        inventory.onChange += UpdateMovementAttributes;
        UpdateMovementAttributes();

        rb = GetComponent<Rigidbody>();
        if (playerCamera == null) { playerCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0]; }
    }

    private void GameInput_OnPuke()
    {
        if (inventory.isEmpty) { return; }

        Consumable itemToPlace = inventory.PopItem();

        var pukeDir = transform.forward;
        itemToPlace.PlaceAt(transform.position + pukeDir*pukeDistance);
    }

    private void GameInput_OnEat()
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

            var consumableData = hitObject.GetComponent<Consumable>();
            if (consumableData == null || !consumableData.isConsumable) { return; }

            inventory.PushItem(consumableData);
            consumableData.SetState(ItemState.beingConsumed);
        }
    }

    public void GameInput_ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = gameInput.movementInput;
        moveDir = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0) * new Vector3(inputVector.x, 0, inputVector.y);

        float turnSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * turnSpeed);
    }

    private void FixedUpdate()
    {
        rb.velocity = moveDir * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }



    private void UpdateMovementAttributes()
    {
        //TODO: make values dynamic (would be more intuitive); alter mass and drag instead? (makes a difference for collisions and falling)

        var totalItemMass = inventory.totalMass;

        moveSpeed = baseMoveSpeed - totalItemMass;
    }

}