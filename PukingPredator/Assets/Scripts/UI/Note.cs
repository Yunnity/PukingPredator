using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Note : MonoBehaviour
{
    /// <summary>
    /// The alpha of the note visuals.
    /// </summary>
    private float alpha
    {
        get => _alpha;
        set
        {
            _alpha = value;
            containerCanvasGroup.alpha = Mathf.Clamp01(value);
        }
    }
    private float _alpha = 0f;

    /// <summary>
    /// The tint/color of the note.
    /// </summary>
    [SerializeField]
    private Color color = Color.white;

    /// <summary>
    /// The container for the note content.
    /// </summary>
    protected GameObject container;
    private CanvasGroup containerCanvasGroup;
    private float containerHeight = 50f;
    private float containerPivotX = 0.5f;
    private float containerPivotY = 0.3f;

    /// <summary>
    /// Delay in seconds before making the note appear. Intended for tutorials.
    /// </summary>
    [SerializeField]
    private float delay = 0f;

    /// <summary>
    /// The rate that the image fades in and out.
    /// </summary>
    private float fadeSpeed = 4f;

    /// <summary>
    /// If the note should be seen currently.
    /// </summary>
    protected bool visible
    {
        get => _visible;
        set
        {
            if (value == _visible) { return; }
            _visible = value;

            StopAllCoroutines(); // Stop any ongoing fading in/out

            var targetAlpha = _visible ? 1f : 0f;
            StartCoroutine(FadeAlpha(targetAlpha));
        }
    }
    private bool _visible = false;



    protected virtual void Awake()
    {
        CreateContainer();

        alpha = 0f; // Default not visible

        //make sure the collider is set to trigger if there is one.
        var collider = GetComponent<Collider>();
        if (collider != null) { collider.isTrigger = true; }
    }

    private void Start()
    {
        if (color != Color.white) { SetContainerTint(color); }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(GameTag.player))
        {
            visible = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(GameTag.player))
        {
            visible = false;
        }
    }



    /// <summary>
    /// Create an Image component dynamically.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="size"></param>
    public GameObject AddImage(Sprite sprite, Vector2 size)
    {
        // Create a new GameObject for the image
        GameObject imageObject = new GameObject("ImageElement");
        imageObject.transform.SetParent(container.transform, false);

        // Add Image component
        Image imageComponent = imageObject.AddComponent<Image>();
        imageComponent.sprite = sprite;
        imageComponent.preserveAspect = true; // Keeps the original aspect ratio

        // Set RectTransform size
        RectTransform rectTransform = imageComponent.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size; // Set to the desired width & height

        // Optionally, add a ContentSizeFitter if you want it to adjust dynamically
        ContentSizeFitter fitter = imageObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return imageObject;
    }

    /// <summary>
    /// Create a Text component dynamically.
    /// </summary>
    protected GameObject AddText(string text)
    {
        // Create new Text object
        GameObject textObject = new GameObject("TextElement");
        textObject.transform.SetParent(container.transform, false);

        // Add TextMeshProUGUI component
        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.alignment = TextAlignmentOptions.Center;

        // Enable Auto-Size for dynamic font scaling
        textComponent.enableAutoSizing = true;
        textComponent.fontSizeMin = 10f;
        textComponent.fontSizeMax = 36f;

        // Set RectTransform height while letting width auto-adjust
        RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, 50f); // Fixed height, width will adjust dynamically

        // Add a ContentSizeFitter to dynamically resize the width
        ContentSizeFitter fitter = textObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained; // Keep height fixed

        return textObject;
    }

    private void CreateContainer()
    {
        var canvas = GameObject.Find("Canvas");

        // Create a new container GameObject and make it a child of the Canvas
        container = new GameObject("TextContainer");
        container.transform.SetParent(canvas.transform, false);

        // Add RectTransform for positioning
        RectTransform rectTransform = container.AddComponent<RectTransform>();
        rectTransform.pivot = new Vector2(containerPivotX, containerPivotY);
        rectTransform.anchorMin = new Vector2(containerPivotX, containerPivotY);
        rectTransform.anchorMax = new Vector2(containerPivotX, containerPivotY);
        rectTransform.sizeDelta = new Vector2(Screen.width * 0.9f, containerHeight);
        rectTransform.anchoredPosition = Vector2.zero;

        // Add HorizontalLayoutGroup for automatic arrangement
        HorizontalLayoutGroup layoutGroup = container.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.spacing = 10f;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;

        // Add CanvasGroup for controlling alpha
        containerCanvasGroup = container.AddComponent<CanvasGroup>();
    }

    /// <summary>
    /// Coroutine to fade the image to the target alpha over time.
    /// </summary>
    /// <param name="targetAlpha"></param>
    /// <returns></returns>
    IEnumerator FadeAlpha(float targetAlpha)
    {
        while (!Mathf.Approximately(alpha, targetAlpha))
        {
            // Handle the delay if trying to go up.
            if (targetAlpha > alpha)
            {
                while (delay > 0)
                {
                    delay -= Time.deltaTime;
                    yield return null;
                }
            }

            // Gradually adjust the alpha based on fadeSpeed and deltaTime
            alpha = Mathf.MoveTowards(alpha, targetAlpha, fadeSpeed * Time.deltaTime);

            yield return null;
        }
    }

    public void SetContainerTint(Color tintColor)
    {
        foreach (Transform child in container.transform)
        {
            // Tint TextMeshProUGUI elements
            TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.color = tintColor;
            }

            // Tint Image elements
            Image imageComponent = child.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.color = tintColor;
            }
        }
    }
}
