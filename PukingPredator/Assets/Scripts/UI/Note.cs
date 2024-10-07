using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    /// <summary>
    /// The range that the note will appear inside.
    /// </summary>
    [SerializeField]
    private float detectionRange = 2f;

    /// <summary>
    /// The rate that the image fades in and out.
    /// </summary>
    [SerializeField]
    private float fadeSpeed = 4f;

    /// <summary>
    /// If the player was nearby the last time a check was performed.
    /// </summary>
    private bool isPlayerNearby = false;

    /// <summary>
    /// To store the original alpha of the image.
    /// </summary>
    private float originalAlpha;

    /// <summary>
    /// Reference to the player to check for proximity.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// The image shown by the note.
    /// </summary>
    [SerializeField]
    private Sprite sprite;

    /// <summary>
    /// A reference to the image this note is for.
    /// </summary>
    private Image UIImage;



    void Start()
    {
        CreateImageComponent();

        player = GameObject.Find("Player");
    }

    void Update()
    {
        // Check the distance between the player and this object
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // If player is within the trigger distance, gradually fade in
        if (distance < detectionRange && !isPlayerNearby)
        {
            isPlayerNearby = true;
            StopAllCoroutines(); // Stop any ongoing fading out
            StartCoroutine(FadeImage(originalAlpha)); // Fade in to alpha = 1 * originalAlpha
        }
        // If player is farther than the trigger distance, gradually fade out
        else if (distance >= detectionRange && isPlayerNearby)
        {
            isPlayerNearby = false;
            StopAllCoroutines(); // Stop any ongoing fading in
            StartCoroutine(FadeImage(0f)); // Fade out to alpha = 0
        }

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
        SetImageAlpha(0f);
    }

    /// <summary>
    /// Coroutine to fade the image to the target alpha over time.
    /// </summary>
    /// <param name="targetAlpha"></param>
    /// <returns></returns>
    IEnumerator FadeImage(float targetAlpha)
    {
        float currentAlpha = UIImage.color.a;

        // Continue adjusting alpha until it reaches the target alpha
        while (!Mathf.Approximately(currentAlpha, targetAlpha))
        {
            // Gradually adjust the alpha based on fadeSpeed and deltaTime
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);

            // Set the new alpha value to the image
            SetImageAlpha(currentAlpha);

            yield return null;
        }
    }

    /// <summary>
    /// Helper function to set the alpha of the UI Image.
    /// </summary>
    /// <param name="alpha"></param>
    void SetImageAlpha(float alpha)
    {
        Color color = UIImage.color;
        color.a = alpha;
        UIImage.color = color;
    }
}
