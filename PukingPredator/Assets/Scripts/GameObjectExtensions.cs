using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// Centeres the origin/pivot of the object by shifting the colliders,
    /// visuals, and children. This may not work for objects with arbitrary
    /// components attached, use with caution.
    /// </summary>
    /// <param name="gameObject"></param>
    public static void CenterOrigin(this GameObject gameObject)
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();

        if (colliders.Length == 0) { return; }

        // Calculate the average center of all colliders
        Vector3 center = Vector3.zero;
        foreach (var col in colliders)
        {
            center += col.bounds.center;
        }
        center /= colliders.Length;

        // Convert world position to local position
        Vector3 localCenter = gameObject.transform.InverseTransformPoint(center);

        // Shift all children and components to compensate for pivot change
        foreach (Transform child in gameObject.transform)
        {
            child.localPosition -= localCenter;
        }

        // Offset the mesh vertices
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter && meshFilter.sharedMesh)
        {
            meshFilter.sharedMesh = OffsetMesh(meshFilter.sharedMesh, -localCenter);
        }

        foreach (Collider col in colliders)
        {
            if (col is BoxCollider box)
            {
                box.center -= localCenter;
            }
            else if (col is SphereCollider sphere)
            {
                sphere.center -= localCenter;
            }
            else if (col is CapsuleCollider capsule)
            {
                capsule.center -= localCenter;
            }
        }

        // Adjust object position to maintain visual placement
        gameObject.transform.position += gameObject.transform.TransformVector(localCenter);
    }
    /// <summary>
    /// Helper for CenterOrigin.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    private static Mesh OffsetMesh(Mesh original, Vector3 offset)
    {
        if (!original) return null;

        Mesh newMesh = GameObject.Instantiate(original);
        Vector3[] vertices = newMesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += offset;
        }

        newMesh.vertices = vertices;
        newMesh.RecalculateBounds();
        return newMesh;
    }

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