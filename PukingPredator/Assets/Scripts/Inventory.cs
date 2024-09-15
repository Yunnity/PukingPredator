using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Stack to store items in the inventory
    private Stack<Item> items;

    // Start is called before the first frame update
    void Start()
    {
        items = new Stack<Item>();
    }

    public bool isFull()
    {
        return items.Count >= 2 ;
    }
    // Method to add an item to the inventory
    public void AddItem(Item item)
    {
        items.Push(item);
        Debug.Log("Item added: " + item.name);
    }

    // Method to remove the last added item from the inventory
    public Item RemoveItem()
    {
        if (items.Count > 0)
        {
            Item removedItem = items.Pop();
            Debug.Log("Item removed: " + removedItem.name);
            return removedItem;
        }
        else
        {
            Debug.Log("Inventory is empty!");
            return null;
        }
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
