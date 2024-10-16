using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class InventoryUI : MonoBehaviour
{
    public TMP_Text inventoryText;
    string currColor;

    public void UpdateText(float currInventoryCount, float maxInventoryCount)
    {
        float currRatio = currInventoryCount / maxInventoryCount;
        if (currRatio < 0.5) currColor = "green";
        else if (currInventoryCount == maxInventoryCount) currColor = "red";
        else currColor = "yellow";
        inventoryText.text = $"<color={currColor}>Inventory: {currInventoryCount}/{maxInventoryCount}</color>";
    }
}
