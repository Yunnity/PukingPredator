using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    /// <summary>
    /// Image used to display the stomach.
    /// </summary>
    [SerializeField]
    private Image stomachImage;

    /// <summary>
    /// List of stomach sprites that indicate fullness.
    /// </summary>
    [SerializeField]
    private List<Sprite> stomachSprites;

    /// <summary>
    /// List of thumbnail images displayed in the inventory.
    /// </summary>
    [SerializeField]
    private List<Image> thumbnailImages;

    /// <summary>
    /// Used to prevent images from showing a solid color when empty.
    /// </summary>
    private Color transparentColor = new Color(0, 0, 0, 0);



    public void UpdateImages(List<Sprite> icons)
    {
        stomachImage.sprite = stomachSprites[icons.Count];

        List<(Image image, Sprite sprite)> pairedList = thumbnailImages
            .Select((value, index) => (value, index < icons.Count ? icons[index] : null))
            .ToList();

        foreach (var pair in pairedList)
        {
            var image = pair.image;
            var sprite = pair.sprite;

            image.sprite = sprite;
            image.color = sprite == null ? transparentColor : Color.white;
        }
    }
}
