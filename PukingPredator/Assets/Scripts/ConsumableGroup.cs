using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class ConsumableGroup : Consumable
{
    /// <summary>
    /// Moves the group items to the position and reactivates them.
    /// </summary>
    /// <param name="position"></param>
    public override void PlaceAt(Vector3 position)
    {
        if (state != ItemState.inInventory) { return; }
        gameObject.transform.localScale = initialScale;

        foreach (Transform childTransform in transform)
        {
            Consumable childConsumable = childTransform.GetComponent<Consumable>();
            if (childConsumable != null)
            {
                
                Vector3 childPosition = position + childTransform.localPosition;

                var previousAngles = childTransform.eulerAngles;
                childTransform.eulerAngles = new Vector3(0, previousAngles.y, 0);
                childTransform.position = childPosition;

                childConsumable.SwapToLayer();
            }
        }


        SetState(ItemState.inWorld);
        UnGroup();
    }


    /// <summary>
    /// Makes each child of the group its own consumable object
    /// </summary>
    public void UnGroup()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform childTransform = transform.GetChild(i);
            childTransform.SetParent(null);
        }

        Destroy(gameObject);
    }
}
