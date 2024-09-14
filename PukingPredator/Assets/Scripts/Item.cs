using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private GameObject prefab;
    private Vector3 position;
    private Quaternion rotation;

    void PlaceItem()
    {
        Instantiate(prefab, position, rotation);
    }

    public void Initialize(GameObject newPrefab, Vector3 newPosition, Quaternion newRotation)
    {
        prefab = newPrefab;
        position = newPosition;
        rotation = newRotation == default ? Quaternion.identity : newRotation;
    }
}
