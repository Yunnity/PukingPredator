using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    /// <summary>
    /// If the inventory is empty.
    /// </summary>
    public bool isEmpty
    {
        get => items.Count == 0;
    }

    /// <summary>
    /// If the inventory is full.
    /// </summary>
    public bool isFull
    {
        get => items.Count >= maxCount;
    }

    /// <summary>
    /// The number of items in the inventory.
    /// </summary>
    public int itemCount
    {
        get => items.Count;
    }

    /// <summary>
    /// Stack to store the items in the inventory.
    /// </summary>
    [SerializeField]
    private List<Item> items = new();

    /// <summary>
    /// The max number of items that can be held at once.
    /// </summary>
    [SerializeField]
    private int maxCount = 2;

    /// <summary>
    /// Triggered whenever items enter/leave the inventory or decay. This will
    /// specifically trigger after the change has taken effect.
    /// </summary>
    //TODO: make this trigger when the items decay
    public event Action onChange;

    /// <summary>
    /// The mass of all items in the inventory.
    /// </summary>
    public float totalMass => items.Select(i => i.mass).Sum();



    /// <summary>
    /// Add an item to the inventory
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public void PushItem(Item item)
    {
        items.Insert(0, item);
        Debug.Log($"Item added: {item.name}");

        onChange?.Invoke();
    }

    /// <summary>
    /// Remove an item from the inventory and return it.
    /// </summary>
    /// <returns>The last item added to the inventory.</returns>
    public Item PopItem()
    {
        if (isEmpty) { return null; }

        var topOfStack = items[0];

        if (topOfStack.state != ItemState.inInventory) { return null; }

        items.RemoveAt(0);

        onChange?.Invoke();

        return topOfStack;
    }


    public bool ReplaceItem(Item current, Item newItem)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == current)
            {
                if (newItem == null)
                {
                    items.Remove(current);
                }
                else
                {
                    items[i] = newItem;
                }

                return true;
            }
        }

        return false;
    }
}