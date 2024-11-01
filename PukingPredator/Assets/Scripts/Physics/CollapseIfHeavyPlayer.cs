using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsBehaviour))]
public class CollapseIfHeavyPlayer : MonoBehaviour
{

    private PhysicsBehaviour pb;

    /// <summary>
    /// If player is greater than or equal to this mass when they collide with the structure, collapse the structure.
    /// </summary>
    [SerializeField]
    private float massThreshold = 5f;

    // Start is called before the first frame update
    void Start()
    {
        pb = GetComponent<PhysicsBehaviour>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(GameTag.player))
        {
            Player player= collision.collider.GetComponent<Player>();
            if (player != null && player.GetMass() >= massThreshold)
            {
                pb.EnablePhysics();
            }
        }
    }
}
