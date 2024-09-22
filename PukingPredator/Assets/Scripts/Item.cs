using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Inventory inventory;

    public GameObject prefab;
    private Rigidbody rigidBody;

    private Vector3 position;
    private Quaternion rotation;

    private Vector3 playerPos;
    private Vector3 initialScale;
    
    public bool isCollecting = false;
    float collectionTimer = 0f;
    Timer timer;

    public void StartDecay(object sender, System.EventArgs e)
    {
        if (prefab == null) { return; }
        ItemReplace replacement = prefab.GetComponent<ItemReplace>();
        Item nextItem = null;

        // Checks if theres an item that will replace the current one after the decay
        if (replacement != null)
        {
            nextItem = replacement.GetNext();
            nextItem.inventory = inventory;
        }

        // Tries replacing the item in teh inventory with the new decayed item
        if (!inventory.ReplaceItem(this, nextItem))
        {
            // This means the item did not decay in the player it decayed outside
            if (replacement)
            {
                replacement.Replace();
            }
            else
            {
                prefab.SetActive(false);
            };
        };
    }

    public void PlaceItem()
    {
        if (isCollecting) {  return; }
        prefab.SetActive(true);
    }

    public void MoveItem(Vector3 position)
    {
        prefab.transform.position = position;
    }

    public void Initialize(GameObject newPrefab, Vector3 newPosition, Quaternion newRotation)
    {
        prefab = newPrefab;
        initialScale = prefab.transform.localScale;
        rigidBody = prefab.GetComponent<Rigidbody>();

        position = newPosition;
        rotation = newRotation == default ? Quaternion.identity : newRotation;
    }

    public void Collect(Vector3 pos)
    {
        playerPos = pos;
        isCollecting = true;

        timer = gameObject.AddComponent<Timer>();
        timer.Init(5f);
        timer.OnTimerComplete += StartDecay;
        timer.StartTimer();
    }

    public void Update()
    {
        if (isCollecting)
        {
            collectionTimer += Time.deltaTime;
            float lerpFactor = collectionTimer / 0.6f;

            prefab.transform.position = Vector3.Lerp(prefab.transform.position, playerPos, lerpFactor);
            prefab.transform.localScale = Vector3.Lerp(prefab.transform.localScale, Vector3.zero, lerpFactor);

            if (lerpFactor >= 0.8f)
            {
                collectionTimer = 0f;
                prefab.SetActive(false);

                prefab.transform.localScale = initialScale;
                prefab.transform.rotation = Quaternion.identity;

                if (rigidBody != null)
                {
                    rigidBody.velocity = Vector3.zero;
                    rigidBody.angularVelocity = Vector3.zero;
                }

                isCollecting = false;
            }
        }
    }
}
