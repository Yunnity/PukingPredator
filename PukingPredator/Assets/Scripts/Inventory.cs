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
    /// Checks if the inventory contains the given item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool ContainsItem(Item item)
    {
        return items.Contains(item);
    }

    /// <summary>
    /// Handles behaviour for whenever an item is added.
    /// </summary>
    /// <param name="item"></param>
    private void ItemAdded(Item item)
    {
        item.inventory = this;
        onChange?.Invoke();
    }

    /// <summary>
    /// Handles behaviour for whenever an item is removed.
    /// </summary>
    /// <param name="item"></param>
    private void ItemRemoved(Item item)
    {
        item.inventory = null;
        onChange?.Invoke();
    }

    /// <summary>
    /// Add an item to the inventory
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public void PushItem(Item item)
    {
        items.Insert(0, item);
        Debug.Log($"Item added: {item.name}");

        ItemAdded(item);
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

        ItemRemoved(topOfStack);

        return topOfStack;
    }

    /// <summary>
    /// Removes the item from the inventory.
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(Item item)
    {
        items.Remove(item);

        ItemAdded(item);
    }

    /// <summary>
    /// Replaces an item with another.
    /// </summary>
    /// <param name="oldItem"></param>
    /// <param name="newItem"></param>
    /// <returns></returns>
    public bool ReplaceItem(Item oldItem, Item newItem)
    {
        if (!items.Contains(oldItem)) { return false; }

        if (newItem == null)
        {
            items.Remove(oldItem);
        }
        else
        {
            var index = items.IndexOf(oldItem);
            items[index] = newItem;

            ItemAdded(newItem);
        }

        ItemRemoved(oldItem);

        return true;
    }
}