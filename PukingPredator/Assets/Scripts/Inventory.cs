using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    /// The prefab used to create item slots in the inventory UI.
    /// </summary>
    public GameObject itemUIPrefab;

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
    /// The panel that the inventory is contained in.
    /// </summary>
    public GameObject panel;

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

    /// <summary>
    /// Changes the items that are currently displayed.
    /// </summary>
    public void UpdateUI()
    {
        const string ITEM_SLOT_ID = "ItemSlotUI";

        // Clear the items other than the label for the inventory
        foreach (Transform child in panel.transform)
        {
            if (child.gameObject.name == ITEM_SLOT_ID)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (var item in items.AsEnumerable().Reverse())
        {
            GameObject newItem = Instantiate(itemUIPrefab, panel.transform);
            newItem.name = ITEM_SLOT_ID;

            var itemSlotLabel = newItem.GetComponent<TextMeshProUGUI>();
            itemSlotLabel.text = item.instanceName;
        }
    }

}
