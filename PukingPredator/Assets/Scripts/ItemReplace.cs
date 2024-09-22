using UnityEngine;

public class ItemReplace : MonoBehaviour
{
    /// <summary>
    /// The prefab for the instance that replaces the current instance.
    /// </summary>
    public GameObject nextPrefab { get; private set; }



    /// <summary>
    /// Replaces the current prefab with the specified next prefab.
    /// </summary>
    /// <returns>If the replacement was successful</returns>
    public bool Replace()
    {
        if (nextPrefab is null) { return false; }

        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        GameObject newPrefab = Instantiate(nextPrefab, position, rotation);
        newPrefab.SetActive(true);

        Destroy(gameObject);

        return true;
    }
}
