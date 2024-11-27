using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerNoteText : MonoBehaviour
{

    [SerializeField]
    private GameObject PlayerObj;

    private Player player;

    private Inventory inventory;

    private TMP_Text UIText;

    private Vector2 margin = Vector2.one * 20f;

    private float destroyTime = 3f;

    private bool showPukeText = true;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerObj.GetComponent<Player>();
        inventory = player.inventory;
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory.isFull && showPukeText)
        {
            showPukeText = false;
            CreateTextComponent("You are full. Press [RMB/RT] to Puke", Color.red);
        }
    }

    /// <summary>
    /// Create the Text component dynamically.
    /// </summary>
    void CreateTextComponent(string text, Color color)
    {
        var canvas = GameObject.Find("Canvas");

        // Create a new GameObject and make it a child of the Canvas
        GameObject textObject = new GameObject("TriggerText");
        textObject.transform.SetParent(canvas.transform, false); // Ensure it stays relative to the canvas

        // Add a Text component to the GameObject
        UIText = textObject.AddComponent<TextMeshProUGUI>();
        UIText.text = text;
        UIText.color = color;
        UIText.alignment = TextAlignmentOptions.Center;

        RectTransform rectTransform = UIText.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0.5f, 0.3f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.3f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.3f);
        rectTransform.sizeDelta = new Vector2(Screen.width * 0.9f, rectTransform.sizeDelta.y);
        rectTransform.anchoredPosition = Vector2.zero;

        Destroy(textObject, destroyTime);
    }
}
