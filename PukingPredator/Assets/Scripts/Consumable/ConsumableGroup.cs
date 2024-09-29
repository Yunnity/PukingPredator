using UnityEngine;

public class ConsumableGroup : Consumable
{
    protected override void Awake()
    {
        base.Awake();

        //break the group down the moment it is in the world
        stateEvents[ItemState.inWorld].onUpdate += Ungroup;

    }



    /// <summary>
    /// Deletes itself after breaking the parent-child relationship with its children.
    /// </summary>
    public void Ungroup()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform childTransform = transform.GetChild(i);
            childTransform.SetParent(null);

            var childRB = childTransform.gameObject.GetComponent<Rigidbody>();
            if (childRB != null)
            {
                childRB.velocity = Vector3.zero;
                childRB.angularVelocity = Vector3.zero;
            }
        }
        Destroy(gameObject);
    }
}
