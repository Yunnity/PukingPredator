using UnityEngine;

public class ItemReplace : MonoBehaviour
{
    public GameObject nextPrefab;

    public Item GetNext()
    {
        GameObject replaceItemObject = new GameObject("ReplaceItem");
        Item replaceItem = replaceItemObject.AddComponent<Item>();

        GameObject replaceObject = Instantiate(nextPrefab, transform.position, transform.rotation);
        replaceObject.SetActive(false);

        replaceItem.Initialize(replaceObject, transform.position, transform.rotation);
        return replaceItem;
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
            Destroy(gameObject);

            return true;
        }

        return false;
    }
}
