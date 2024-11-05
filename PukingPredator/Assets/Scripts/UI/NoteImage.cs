using UnityEngine;
using UnityEngine.UI;

public class NoteImage : Note
{
    /// <summary>
    /// To store the original alpha of the image.
    /// </summary>
    private float originalAlpha;

    /// <summary>
    /// The image shown by the note.
    /// </summary>
    [SerializeField]
    private Sprite sprite;

    /// <summary>
    /// A reference to the image this note is for.
    /// </summary>
    private Image UIImage;



    private void Awake()
    {
        CreateImageComponent();
    }



    /// <summary>
    /// Create the Image component dynamically.
    /// </summary>
    void CreateImageComponent()
    {
        var canvas = GameObject.Find("Canvas");

        // Create a new GameObject and make it a child of the Canvas
        GameObject imageObject = new GameObject("ProximityImage");
        imageObject.transform.SetParent(canvas.transform, false); // Ensure it stays relative to the canvas

        // Add an Image component to the GameObject
        UIImage = imageObject.AddComponent<Image>();
        UIImage.sprite = sprite;

        RectTransform rectTransform = UIImage.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0);
        //TODO: pull this out into a property? fix it so it scales correctly?
        //TODO: should this be done with sizeDelta? thats what i saw online, but it didnt work
        rectTransform.localScale = Vector3.one * 6f;

        // Store the original alpha of the image
        originalAlpha = UIImage.color.a;
    }

    /// <summary>
    /// Helper function to set the alpha of the UI Image.
    /// </summary>
    /// <param name="alpha"></param>
    protected override void SetAlpha(float alpha)
    {
        Color color = UIImage.color;
        color.a = alpha * originalAlpha;
        UIImage.color = color;
    }
}
