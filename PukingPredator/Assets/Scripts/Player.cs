using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Inventory inventory;
    private Vector3 lastDir;

    const float SPEED = 20;

    private void Start()
    {
        gameInput.onInteractAction += GameInput_OnInteract;
        gameInput.onPukeAction += GameInput_OnPuke;
    }

    private void GameInput_OnPuke(object sender, System.EventArgs e)
    {
        if (!inventory.IsEmpty())
        {
            Item itemToPlace = inventory.RemoveItem();
            itemToPlace.MoveItem(transform.position + lastDir);
            itemToPlace.PlaceItem();
        }
    }

    private void GameInput_OnInteract(object sender, System.EventArgs e)
    {
        // Define the ray, starting from the player's position, shooting forward
        Ray ray = new Ray(transform.position, lastDir);
        RaycastHit hit;
        float sphereRadius = 0.5f; // Adjust as needed

        // Perform the sphere cast
        if (Physics.SphereCast(ray, sphereRadius, out hit, Mathf.Infinity)) // 5f is the maximum ray distance
        {
            // Get the GameObject that was hit
            GameObject hitObject = hit.collider.gameObject;

            // Log the object hit
            Debug.Log("Hit object: " + hitObject.name);

            GameObject newItemObject = new GameObject("NewItem");  // Create a new empty GameObject
            Item newItem = newItemObject.AddComponent<Item>();     // Add the Item component

            // Initialize the item with properties from the hit object
            Vector3 itemPosition = hitObject.transform.position;
            Quaternion itemRotation = hitObject.transform.rotation;

            newItem.Initialize(hitObject.gameObject, itemPosition, itemRotation);

            // Add the newly created item to the inventory
            inventory.AddItem(newItem);

            // Optional: Destroy the hit object after turning it into an inventory item
            hitObject.SetActive(false);
        }
        else
        {
            Debug.Log("No object hit.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = gameInput.GetInputVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        transform.position += moveDir * SPEED * Time.deltaTime;

        float turnSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * turnSpeed);

        if (moveDir != Vector3.zero)
        {
            lastDir = moveDir;
        }
    }
}
