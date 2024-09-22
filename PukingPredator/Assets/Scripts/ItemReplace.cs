using UnityEngine;

public class ItemReplace : MonoBehaviour
{
    [SerializeField]
    private GameObject nextPrefab;

    /// <summary>
    /// Returns the prefab that will replace the current one
    /// </summary>
    public GameObject GetNext()
    {
        return nextPrefab;
    }

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
