using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    public Image inventoryImage;
    public List<Image> thumbnailImages;

    public List<Sprite> sprites;
    private List<Sprite> itemSprites;

    private Color transparent;
    private Color opaque;

    public void Awake()
    {
        transparent = new Color(0, 0, 0, 0);
        opaque = Color.white;
        itemSprites = new List<Sprite>();

        // Ensures that when the Image is empty it doesnt display a solid color.
        foreach (var image in thumbnailImages)
        {
            image.color = transparent;
        }
    }

    public void UpdateImage(int currInventoryCount, Sprite thumbnail)
    {
        inventoryImage.sprite = sprites[currInventoryCount];
        if (currInventoryCount > itemSprites.Count)
        {
            itemSprites.Add(thumbnail);
        } else
        {
            if (itemSprites.Count != 0) itemSprites.RemoveAt(itemSprites.Count - 1);
        }

        for (int i = 0; i < thumbnailImages.Count; i++)
        {
            Image image = thumbnailImages[i];
            int itemIndex = itemSprites.Count - i - 1;

            if (itemIndex < 0) image.color = transparent;
            else
            {
                if (itemSprites[itemIndex] == null) image.color = transparent;
                else
                {
                    image.color = opaque;
                    image.sprite = itemSprites[itemIndex];
                }
            }
        }
    }
}
