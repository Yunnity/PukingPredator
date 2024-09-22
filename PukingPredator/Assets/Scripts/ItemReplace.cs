using UnityEngine;

public class ItemReplace : MonoBehaviour
{
    /// <summary>
    /// The prefab for the instance that replaces the current instance.
    /// </summary>
    public GameObject nextPrefab { get; private set; }



    /// <summary>
    /// Replaces the current prefab with the specified next prefab.
    /// Returns true if the replacement was successful; otherwise, false.
    /// </summary>
    public bool Replace()
    {
        if (nextPrefab != null)
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            GameObject newPrefab = Instantiate(nextPrefab, position, rotation);
            newPrefab.SetActive(true);

            Destroy(gameObject);

            return true;
        }

        return false;
    }
}
