using System.Collections.Generic;
using UnityEngine;

public class CameraCulling : MonoBehaviour
{
    public Transform player;

    private List<GameObject> previousObstructed;
    List<GameObject> currentHits;

    private RaycastHit hit;

    private void Start()
    {
        previousObstructed = new List<GameObject>();
        currentHits = new List<GameObject>();
    }

    void LateUpdate()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceBetween = Vector3.Distance(player.position, transform.position);


        RaycastHit[] hits = Physics.RaycastAll(transform.position, directionToPlayer, distanceBetween);
        currentHits.Clear();

        foreach (RaycastHit hit in hits)
        {

            GameObject hitObject = hit.collider.gameObject;
            if (hitObject == player) continue;

            // Stores the currenthits in a list so it can easily be checked for non-hit previous objects
            currentHits.Add(hitObject);
            if (previousObstructed.Contains(hitObject)) continue;

            SeTransparencyRecursively(hitObject, (float)0.15);
            previousObstructed.Add(hitObject);  
        }
        
        for (int i = previousObstructed.Count - 1; i >= 0; i--)
        {
            GameObject previous = previousObstructed[i];
            if (!currentHits.Contains(previous))
            {
                SeTransparencyRecursively(previous, 1);
                previousObstructed.RemoveAt(i);
            }
        }
    }

    void SeTransparencyRecursively(GameObject obj, float transparency)
    {
        MakeTransparent(obj, transparency);
        foreach (Transform child in obj.transform)
        {
            SeTransparencyRecursively(child.gameObject, transparency);
        }
    }

    void MakeTransparent(GameObject obj, float transparencyLevel)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        
        if (renderer != null)
        {
            foreach (Material mat in renderer.materials)
            {
                if (!mat.HasProperty("_Color")) continue;
                Color color = mat.color;
                color.a = transparencyLevel;
                mat.color = color;
            }
        }
    }
}
