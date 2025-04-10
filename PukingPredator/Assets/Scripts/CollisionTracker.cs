using System;
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

    /// <summary>
    /// A list of functions to filter the collisions. The functions should
    /// return true if the collision is acceptable and false if it should be
    /// filtered.
    /// </summary>
    private List<Func<Collider, bool>> filters = new();

    /// <summary>
    /// If the tracker should ignore trigger colliders.
    /// </summary>
    [SerializeField]
    private bool ignoreTriggerColliders = false;

    /// <summary>
    /// Triggers when a new collision occurs.
    /// </summary>
    private Action onCollision;

    /// <summary>
    /// Triggers when player leaves collider.
    /// </summary>
    private Action onLeave;

    /// <summary>
    /// If the tracker should only check for player collisions.
    /// </summary>
    [SerializeField]
    private bool onlyPlayerCollisions = false;



    private void OnTriggerEnter(Collider other)
    {
        if (!PassesAllFilters(other)) { return; }

        if (_collisions.Contains(other.gameObject)) { return; }
        
        _collisions.Add(other.gameObject);
        onCollision?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PassesAllFilters(other)) { return; }

        _collisions.Remove(other.gameObject);
        onLeave?.Invoke();
    }

    private void Start()
    {
        if (ignoreTriggerColliders) { AddFilter(NoTriggerCollidersFilter); }
        if (onlyPlayerCollisions) { AddFilter(OnlyPlayerFilter); }
    }

    private void Update()
    {
        // Remove destroyed objects from the list
        _collisions = _collisions.Where(c => c != null).ToList();
    }



    public void SubscribeOnCollision(Action action)
    {
        onCollision += action;
    }

    public void UnsubscribeOnCollision(Action action)
    {
        onCollision -= action;
    }

    public void SubscribeOnLeave(Action action)
    {
        onLeave += action;
    }

    public void UnsubscribeOnLeave(Action action)
    {
        onLeave -= action;
    }

    #region Filters

    /// <summary>
    /// Add a new filter dynamically. The functions should return true if the
    /// collision is acceptable and false if it should be filtered.
    /// </summary>
    /// <param name="filter"></param>
    public void AddFilter(Func<Collider, bool> filter)
    {
        filters.Add(filter);
    }

    /// <summary>
    /// Clear all filters.
    /// </summary>
    public void ClearFilters()
    {
        filters.Clear();
    }

    /// <summary>
    /// Checks if the collider passes all the filters
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    public bool PassesAllFilters(Collider collider)
    {
        foreach (var filter in filters)
        {
            if (!filter(collider))
            {
                return false; // If any filter fails, return false
            }
        }
        return true; // All filters passed
    }

    /// <summary>
    /// Remove a specific filter.
    /// </summary>
    /// <param name="filter"></param>
    public void RemoveFilter(Func<Collider, bool> filter)
    {
        filters.Remove(filter);
    }

    #region Specific Filters

    /// <summary>
    /// A filter for removing trigger colliders.
    /// </summary>
    /// <param name="collision"></param>
    /// <returns></returns>
    private bool NoTriggerCollidersFilter(Collider collider)
    {
        return !collider.isTrigger;
    }

    /// <summary>
    /// A filter for only checking player collisions.
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    private bool OnlyPlayerFilter(Collider collider)
    {
        return collider.CompareTag(GameTag.player);
    }
    #endregion

    #endregion
}
