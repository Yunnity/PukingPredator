using UnityEngine;

public class CameraCulling : MonoBehaviour
{
    public Transform player;
    public LayerMask obstructionMask;
    
    private LayerMask previousLayerMask;
    private GameObject previousObstructed = null;

    private RaycastHit hit;
    private const string OBSTRUCTION = "Obstruction";

    void LateUpdate()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceBetween = Vector3.Distance(player.position, transform.position);
        float cullingDistance = 2 * distanceBetween / 3;


        if (Physics.Raycast(transform.position, directionToPlayer, out hit, cullingDistance))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject == previousObstructed) { return; }
            if (previousObstructed != null && previousObstructed != hitObject)
            {
                SetLayerRecursively(previousObstructed, previousLayerMask);
            }

            previousLayerMask = hitObject.layer;
            SetLayerRecursively(hitObject, LayerMask.NameToLayer(OBSTRUCTION));
            previousObstructed = hitObject;
        }
        else if (previousObstructed != null)
        {
            SetLayerRecursively(previousObstructed, previousLayerMask);
            previousObstructed = null;
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
