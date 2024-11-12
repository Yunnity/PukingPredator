using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Image inventoryImage;
    public List<Sprite> sprites;
    string currColor;

    public void UpdateImage(int currInventoryCount)
    {
        inventoryImage.sprite = sprites[currInventoryCount];
    }
}
