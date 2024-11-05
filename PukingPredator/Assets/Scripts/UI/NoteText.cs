using TMPro;
using UnityEngine;

public class NoteText : Note
{
    private Color color = Color.white;

    private Vector2 margin = Vector2.one * 20f;

    /// <summary>
    /// The text shown by the note.
    /// </summary>
    [SerializeField]
    private string text;

    /// <summary>
    /// A reference to the text this note is for.
    /// </summary>
    private TMP_Text UIText;



    void Awake()
    {
        CreateTextComponent();
    }



    /// <summary>
    /// Create the Text component dynamically.
    /// </summary>
    void CreateTextComponent()
    {
        var canvas = GameObject.Find("Canvas");

        // Create a new GameObject and make it a child of the Canvas
        GameObject textObject = new GameObject("ProximityText");
        textObject.transform.SetParent(canvas.transform, false); // Ensure it stays relative to the canvas

        // Add a Text component to the GameObject
        UIText = textObject.AddComponent<TextMeshProUGUI>();
        UIText.text = text;
        UIText.color = color;
        UIText.alignment = TextAlignmentOptions.Right;

        RectTransform rectTransform = UIText.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(1f, 0f);
        rectTransform.anchorMin = new Vector2(1f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.sizeDelta = new Vector2(Screen.width * 0.9f, rectTransform.sizeDelta.y);
        rectTransform.anchoredPosition = new Vector2(-margin.x, margin.y);
    }

    /// <summary>
    /// Helper function to set the alpha of the UI Image.
    /// </summary>
    /// <param name="alpha"></param>
    protected override void SetAlpha(float alpha)
    {
        UIText.alpha = alpha;
    }
}
