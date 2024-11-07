using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Consumable))]
public class Explosive : MonoBehaviour
{
    // TODO add particles
    [SerializeField]
    private float explosionForce;

    [SerializeField]
    private float explosionRadius;

    [SerializeField]
    private float explosionDelay;
    private const float knockbackForce = 20f;

    // Start is called before the first frame update
    void Start()
    {
        Consumable c = GetComponent<Consumable>();
    }


    public void KnockbackItemsInRadius(Vector3 vec)
    {

        Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f); // Half the size of the box (x, y, z)

        // BoxCast in the direction of the velocity
        RaycastHit[] hits = Physics.BoxCastAll(transform.position + vec.normalized, halfExtents, vec, Quaternion.identity, 2f);

        foreach (RaycastHit hit in hits)
        {
            if ( hit.collider.tag != "Player")
            {
                Rigidbody hitRB = hit.collider.GetComponent<Rigidbody>();
                PhysicsBehaviour pb = hit.collider.GetComponent<PhysicsBehaviour>();
                if (hitRB != null && pb != null)
                {
                    pb.EnablePhysics();
                    hitRB.isKinematic = false;
                    hitRB.AddForce(vec * knockbackForce, ForceMode.Impulse);
                }
            }
        }
    }
}
