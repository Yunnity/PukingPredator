using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// This method will return all descendants of the GameObject.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static List<GameObject> GetDescendants(this GameObject gameObject)
    {
        List<GameObject> descendants = new List<GameObject>();
        GetDescendantsRecursive(gameObject.transform, descendants);
        return descendants;
    }

    /// <summary>
    /// This method will return all descendants of the GameObject and also the game object itself.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static List<GameObject> GetDescendantsAndSelf(this GameObject gameObject)
    {
        List<GameObject> descendants = new List<GameObject>
        {
            gameObject
        };
        GetDescendantsRecursive(gameObject.transform, descendants);
        return descendants;
    }

    /// <summary>
    /// Helper method to recursively find all descendants.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="descendants"></param>
    private static void GetDescendantsRecursive(Transform parent, List<GameObject> descendants)
    {
        foreach (Transform child in parent)
        {
            descendants.Add(child.gameObject);
            GetDescendantsRecursive(child, descendants); // Recursively get the child's descendants
        }
    }
}