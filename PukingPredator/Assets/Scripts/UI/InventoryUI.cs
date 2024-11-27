using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    /// <summary>
    /// Image used to display the stomach
    /// </summary>
    public Image inventoryImage;

    /// <summary>
    /// List of sprites currently assigned to inventory items.
    /// </summary>
    private List<Sprite> itemSprites = new();

    /// <summary>
    /// List of stomach sprites that indicate fullness.
    /// </summary>
    public List<Sprite> sprites;

    /// <summary>
    /// List of thumbnail images displayed in the inventory.
    /// </summary>
    public List<Image> thumbnailImages;

    /// <summary>
    /// Used to prevent images from showing a solid color when empty.
    /// </summary>
    private Color transparentColor = new Color(0, 0, 0, 0);



    public void Awake()
    {
        // Ensures that when the Image is empty it doesnt display a solid color.
        foreach (var image in thumbnailImages)
        {
            image.color = transparentColor;
        }
    }



    public void UpdateImage(int currInventoryCount, Sprite thumbnail)
    {
        inventoryImage.sprite = sprites[currInventoryCount];
        if (currInventoryCount > itemSprites.Count)
        {
            itemSprites.Add(thumbnail);
        }
        else if (itemSprites.Count != 0)
        {
            itemSprites.RemoveAt(itemSprites.Count - 1);
        }

        // Grabs thumbnail image number of items and sets the sprite
        for (int i = 0; i < thumbnailImages.Count; i++)
        {
            Image image = thumbnailImages[i];
            int itemIndex = itemSprites.Count - i - 1;

            if (itemIndex < 0)
            {
                image.color = transparentColor;
            }
            else
            {
                if (itemSprites[itemIndex] == null)
                {
                    image.color = transparentColor;
                }
                else
                {
                    image.sprite = itemSprites[itemIndex];
                    //reset color back to full opacity and no tint
                    image.color = Color.white;
                }
            }
        }
    }
}
