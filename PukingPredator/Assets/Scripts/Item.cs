using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private GameObject prefab;
    private Rigidbody rb;

    private Vector3 position;
    private Quaternion rotation;

    private Vector3 playerPos;
    private Vector3 initialScale;
    
    public bool collect = false;
    float collectionTimer = 0f;

    public void PlaceItem()
    {
        if (collect)
        {
            return;
        }
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
        rb = prefab.GetComponent<Rigidbody>();

        position = newPosition;
        rotation = newRotation == default ? Quaternion.identity : newRotation;
    }

    public void Collect(Vector3 pos)
    {
        playerPos = pos;
        collect = true;
    }

    public void Update()
    {
        if (collect)
        {
            collectionTimer += Time.deltaTime;
            float lerpFactor = collectionTimer / 0.6f;

            prefab.transform.position = Vector3.Lerp(prefab.transform.position, playerPos, lerpFactor);
            prefab.transform.localScale = Vector3.Lerp(prefab.transform.localScale, Vector3.one * 0.3f, lerpFactor);

            if (lerpFactor >= 1f)
            {
                collectionTimer = 0f;
                prefab.SetActive(false);

                prefab.transform.localScale = initialScale;
                prefab.transform.rotation = Quaternion.identity;

                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                collect = false;
            }
        }
    }
}
