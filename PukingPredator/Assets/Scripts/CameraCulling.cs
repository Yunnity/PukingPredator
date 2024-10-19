using System.Collections.Generic;
using UnityEngine;

public class CameraCulling : MonoBehaviour
{
    public Transform player;
    public LayerMask obstructionMask;
    
    private List<LayerMask> previousLayerMasks;
    private List<GameObject> previousObstructed;
    List<GameObject> currentHits;

    private RaycastHit hit;
    private const string OBSTRUCTION = "Obstruction";

    private void Start()
    {
        previousLayerMasks = new List<LayerMask>();
        previousObstructed = new List<GameObject>();
        currentHits = new List<GameObject>();
    }

    void LateUpdate()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceBetween = Vector3.Distance(player.position, transform.position);
        float cullingDistance = 4 * distanceBetween / 5;


        RaycastHit[] hits = Physics.RaycastAll(transform.position, directionToPlayer, cullingDistance);
        currentHits.Clear();

        foreach (RaycastHit hit in hits)
        {

            GameObject hitObject = hit.collider.gameObject;
            if (hitObject == player) continue;

            // Stores the currenthits in a list so it can easily be checked for non-hit previous objects
            currentHits.Add(hitObject);
            if (previousObstructed.Contains(hitObject)) continue;

            previousLayerMasks.Add(hitObject.layer);
            SetLayerRecursively(hitObject, LayerMask.NameToLayer(OBSTRUCTION));
            previousObstructed.Add(hitObject);  
        }
        
        for (int i = previousObstructed.Count - 1; i >= 0; i--)
        {
            GameObject previous = previousObstructed[i];
            if (!currentHits.Contains(previous))
            {
                SetLayerRecursively(previous, previousLayerMasks[i]);
                previousObstructed.RemoveAt(i);
                previousLayerMasks.RemoveAt(i);
            }
        }
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
