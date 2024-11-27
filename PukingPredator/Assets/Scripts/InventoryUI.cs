using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Image inventoryImage;
    public Image thumbnailImage;

    public List<Sprite> sprites;
    public List<Sprite> itemSprites;

    private Color transparent;
    private Color opaque;

    public void Awake()
    {
        transparent = new Color(0, 0, 0, 0);
        opaque = Color.white;

        // Ensures that when the Image is empty it doesnt display a solid color.
        thumbnailImage.color = transparent;
    }

    public void UpdateImage(int currInventoryCount, Sprite thumbnail)
    {
        inventoryImage.sprite = sprites[currInventoryCount];
        if (currInventoryCount > itemSprites.Count)
        {
            thumbnailImage.color = opaque;
            thumbnailImage.sprite = thumbnail;
            itemSprites.Add(thumbnail);
            if (thumbnail == null) thumbnailImage.color = transparent;
        } else
        {
            if (itemSprites.Count != 0) itemSprites.RemoveAt(itemSprites.Count - 1);
            if (currInventoryCount == 0) thumbnailImage.color = transparent;
            else thumbnailImage.sprite = itemSprites[itemSprites.Count - 1];
        }
    }
}
