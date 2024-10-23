using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CollectableTracker : MonoBehaviour
{
    /// <summary>
    /// The main UI component for the collectable data.
    /// </summary>
    private GameObject collectableUI;

    /// <summary>
    /// The panel where the lerp will occur
    /// </summary>
    private GameObject lerpUI;

    /// <summary>
    /// The number of collectables in the level still.
    /// </summary>
    private int remainingCollectables;

    /// <summary>
    /// The number of collectables in total.
    /// </summary>
    private int totalCollectables;

    /// <summary>
    /// The prefab for the collectables UI
    /// </summary>
    [SerializeField]
    private GameObject collectablesUIPrefab;

    /// <summary>
    /// The prefab for the empty slot in the collectables UI
    /// </summary>
    [SerializeField]
    private GameObject emptySlotPrefab;

    /// <summary>
    /// The prefab for the collected slot in the collectables UI
    /// </summary>
    [SerializeField]
    private GameObject collectedSlotPrefab;

    /// <summary>
    /// The prefab for the particle effect when collecting an item
    /// </summary>
    [SerializeField]
    private GameObject particleEffect;

    /// <summary>
    /// The particle system for playing the particle effect
    /// </summary>
    private ParticleSystem effect;


    const float TOP_PADDING = 175f;
    const float LEFT_PADDING = 25f;
    const float ITEM_SPACING = 220f;
    float lerpDuration = 0.5f;

    void Start()
    {
        var canvas = GameObject.Find("Canvas");
        collectableUI = Instantiate(collectablesUIPrefab, canvas.transform);

        // find all collectables
        var collectables = FindObjectsOfType<Collectable>();
        totalCollectables = collectables.Length;
        remainingCollectables = collectables.Length;
        foreach (var collectable in collectables)
        {
            collectable.tracker = this;
        }

        GameObject particleSystem = Instantiate(particleEffect, transform.position, Quaternion.identity);
        effect = particleSystem.GetComponent<ParticleSystem>();

        setupEmptySlots();
    }



    /// <summary>
    /// Called when a collectable has been collected.
    /// </summary>
    public void CollectedOne()
    {
        remainingCollectables--;

        // Instantiate the collected slot at the center of the screen
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        GameObject collectedSlot = Instantiate(collectedSlotPrefab, center, Quaternion.identity, collectableUI.transform);
        StartCoroutine(LerpToPosition(collectedSlot));
    }


    public void emitParticles(Vector3 position)
    {
        effect.transform.position = position;
        effect.Play();

    }

    // Corotuine to lerp the collected item
    IEnumerator LerpToPosition(GameObject collectedSlot)
    {
        RectTransform slotRect = collectedSlot.GetComponent<RectTransform>();
        Vector3 startPosition = slotRect.position;

        float xPos = LEFT_PADDING + ITEM_SPACING * (totalCollectables - remainingCollectables - 1);
        Vector3 targetPosition = new Vector3(xPos, Screen.height - TOP_PADDING, 0);

        float elapsedTime = 0;

        while (elapsedTime < lerpDuration)
        {
            slotRect.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        slotRect.position = targetPosition;
    }

    void setupEmptySlots()
    {
        float xPos = LEFT_PADDING;

        for (int i = 0; i < totalCollectables; i++)
        {
            Vector3 collectablePosition = new Vector3(xPos, Screen.height - TOP_PADDING, 0);
            Instantiate(emptySlotPrefab, collectablePosition, Quaternion.identity, collectableUI.transform);
            xPos += ITEM_SPACING;
        }
    }
}
