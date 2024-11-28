using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    /// <summary>
    /// If the inventory is empty.
    /// </summary>
    public bool isEmpty => items.Count == 0;

    /// <summary>
    /// If the inventory is full.
    /// </summary>
    public bool isFull => items.Count >= maxCount;

    /// <summary>
    /// The number of items in the inventory.
    /// </summary>
    public int itemCount => items.Count;

    /// <summary>
    /// Stack to store the items in the inventory.
    /// </summary>
    [SerializeField]
    private List<Consumable> items = new();

    /// <summary>
    /// The max number of items that can be held at once.
    /// </summary>
    public int maxCount { get; private set; } = 10;

    /// <summary>
    /// Triggered whenever items enter/leave the inventory or decay. This will
    /// specifically trigger after the change has taken effect.
    /// </summary>
    public event Action onChange;

    /// <summary>
    /// The script associated with the inventory UI
    /// </summary>
    [SerializeField]
    private InventoryUI inventoryUI;

    /// <summary>
    /// The object that owns the inventory, ie the player.
    /// </summary>
    public GameObject owner;

    /// <summary>
    /// The mass of all items in the inventory.
    /// </summary>
    public float totalMass => items.Select(i => i.mass).Sum();



    private void Start()
    {
        onChange += UpdateUI;
        UpdateUI();
    }



    /// <summary>
    /// Checks if the inventory contains the given item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool ContainsItem(Consumable item)
    {
        return items.Contains(item);
    }

    /// <summary>
    /// Handles behaviour for whenever an item is added.
    /// </summary>
    /// <param name="item"></param>
    private void ItemAdded(Consumable item)
    {
        item.inventory = this;
        onChange?.Invoke();
    }

    /// <summary>
    /// Handles behaviour for whenever an item is removed.
    /// </summary>
    /// <param name="item"></param>
    private void ItemRemoved(Consumable item)
    {
        item.inventory = null;
        onChange?.Invoke();
    }

    /// <summary>
    /// Returns the item at the index (0 being the top of the
    /// stack).
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Consumable PeekItem(int index = 0)
    {
        return items[index];
    }

    /// <summary>
    /// Add an item to the inventory
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public void PushItem(Consumable item)
    {
        items.Insert(0, item);
        Debug.Log($"Item added: {item.name}");

        ItemAdded(item);
    }
    public void PushItem(GameObject instance)
    {
        PushItem(instance.GetComponent<Consumable>());
    }

    /// <summary>
    /// Remove an item from the inventory and return it.
    /// </summary>
    /// <returns>The last item added to the inventory.</returns>
    public Consumable PopItem()
    {
        var topOfStack = items[0];

        if (topOfStack.state != ItemState.inInventory) { return null; }

        items.RemoveAt(0);

        ItemRemoved(topOfStack);

        return topOfStack;
    }

    /// Removes the item from the inventory.
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(Consumable item)
    {
        items.Remove(item);

        ItemRemoved(item);
    }
    public void RemoveItem(GameObject instance)
    {
        RemoveItem(instance.GetComponent<Consumable>());
    }

    /// <summary>
    /// Replaces an item with another.
    /// </summary>
    /// <param name="oldItem"></param>
    /// <param name="newItem"></param>
    public void ReplaceItem(Consumable oldItem, Consumable newItem)
    {
        var index = items.IndexOf(oldItem);
        items[index] = newItem;

        ItemAdded(newItem);
        ItemRemoved(oldItem);
    }
    public void ReplaceItem(GameObject oldInstance, GameObject newInstance)
    {
        ReplaceItem(oldInstance.GetComponent<Consumable>(), newInstance.GetComponent<Consumable>());
    }

    /// <summary>
    /// Changes the items that are currently displayed.
    /// </summary>
    public void UpdateUI()
    {
        var icons = items.Select(i => i.thumbnail).ToList();
        inventoryUI.UpdateImages(icons);
    }
}
