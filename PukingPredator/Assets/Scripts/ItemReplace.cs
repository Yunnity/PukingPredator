using UnityEngine;

public class ItemReplace : MonoBehaviour
{
    public GameObject nextPrefab;

    /// <summary>
    /// Replaces the current prefab with the specified next prefab.
    /// Returns true if the replacement was successful; otherwise, false.
    /// </summary>
    public bool Replace()
    {
        if (nextPrefab != null)
        {
            Debug.Log(gameObject.activeSelf);
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            GameObject newPrefab = Instantiate(nextPrefab, position, rotation);
            Destroy(gameObject);

            return true;
        }

        return false;
    }
}
