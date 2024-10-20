using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollisionTracker : MonoBehaviour
{
    private List<GameObject> _collisions = new();
    /// <summary>
    /// The list of things currently colliding with the object.
    /// </summary>
    public List<GameObject> collisions => new(_collisions);

    private void OnTriggerEnter(Collider other)
    {
        if (!_collisions.Contains(other.gameObject))
        {
            _collisions.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _collisions.Remove(other.gameObject);
    }

    private void Update()
    {
        // Remove destroyed objects from the list
        _collisions = _collisions.Where(c => c != null).ToList();
    }
}
