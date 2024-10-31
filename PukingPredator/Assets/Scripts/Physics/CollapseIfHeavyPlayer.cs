using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapseIfHeavyPlayer : MonoBehaviour
{

    private PhysicsBehaviour pb;

    [SerializeField]
    private float massThreshold = 5f;

    // Start is called before the first frame update
    void Start()
    {
        pb = GetComponent<PhysicsBehaviour>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Inventory inventory = collision.collider.GetComponentInChildren<Inventory>();
            if (inventory != null && inventory.getMass() > massThreshold)
            {
                pb.EnablePhysics();
            }
        }
    }
}
