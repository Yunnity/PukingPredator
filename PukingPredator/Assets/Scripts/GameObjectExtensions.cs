using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns>The direct children of the GameObject.</returns>
    public static List<GameObject> GetChildren(this GameObject gameObject)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in gameObject.transform)
        {
            children.Add(child.gameObject);
        }
        return children;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns>All descendants of the GameObject.</returns>
    public static List<GameObject> GetDescendants(this GameObject gameObject)
    {
        List<GameObject> descendants = new List<GameObject>();
        GetDescendantsRecursive(gameObject.transform, descendants);
        return descendants;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns>All descendants of the GameObject and also the game object itself.</returns>
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