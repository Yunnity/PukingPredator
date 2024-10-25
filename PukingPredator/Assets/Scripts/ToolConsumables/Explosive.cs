using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    // TODO add particles
    [SerializeField]
    private float explosionForce;

    [SerializeField]
    private float explosionRadius;

    [SerializeField]
    private float explosionDelay;

    // Start is called before the first frame update
    void Start()
    {
        Consumable c = GetComponent<Consumable>();
        c.stateEvents[ItemState.beingPuked].onEnter += Explode;
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        KnockbackItemsInRadius();
        Destroy(gameObject);
    }

    private void Explode()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    private void KnockbackItemsInRadius()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        Vector3 explodePosition = transform.position;

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                PhysicsBehaviour pb = collider.GetComponent<PhysicsBehaviour>();
                if (pb != null)
                {
                    pb.EnablePhysics();
                }
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionForce, explodePosition, 2 * explosionRadius, 0f, ForceMode.VelocityChange);
            }
        }

    }
}
