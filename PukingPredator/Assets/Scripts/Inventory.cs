using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    // Stack to store items in the inventory
    private List<Item> items = new List<Item>();


    public bool isFull()
    {
        return items.Count >= 2 ;
    }
    // Method to add an item to the inventory
    public void AddItem(Item item)
    {
        items.Add(item);
        Debug.Log("Item added: " + item.name);
    }

    // Method to remove the last added item from the inventory
    public Item RemoveItem()
    {
        if (items.Count > 0)
        {
            Item removedItem = items[items.Count - 1];

            // Remove the last item from the list
            items.RemoveAt(items.Count - 1);
            if (removedItem.isCollecting)
            {
                items.Add(removedItem);
                return null;
            }
            return removedItem;
        }
        else
        {
            return null;
        }
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
                } else
                {
                    items[i] = newItem;
                }

                return true;
            }
        }

        return false;
    }

    // Check if the inventory is empty
    public bool IsEmpty()
    {
        return items.Count == 0;
    }

    // Get the current number of items in the inventory
    public int ItemCount()
    {
        return items.Count;
    }
}
