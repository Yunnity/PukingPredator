using UnityEngine;
using UnityEngine.UI;

public class CollectableTracker : MonoBehaviour
{
    /// <summary>
    /// The main UI component for the collectable data.
    /// </summary>
    private GameObject collectableUI;

    /// <summary>
    /// TEMP: The text box used for showing how many collectables are left.
    /// </summary>
    private Text collectablesLeftText;

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
    [SerializeField]
    private GameObject emptySlotPrefab;
    [SerializeField]
    private GameObject collectedSlotPrefab;
    [SerializeField]
    private GameObject particleEffect;
    private ParticleSystem effect;

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

        UpdateUI();
    }



    /// <summary>
    /// Called when a collectable has been collected.
    /// </summary>
    public void CollectedOne()
    {
        remainingCollectables--;
        UpdateUI();
    }

    public void emitParticles(Vector3 position)
    {
        effect.transform.position = position;
        effect.Play();

    }

    private void UpdateUI()
    {
        const string COLLECTABLE_SLOT_ID = "CollectableSlotUI";
        foreach (Transform child in collectableUI.transform)
        {
            if (child.gameObject.name == COLLECTABLE_SLOT_ID)
            {
                Destroy(child.gameObject);
            }
        }

        GameObject newCollectableUI;
        for (int i = 0; i < totalCollectables; i++)
        {
            if (totalCollectables - i > remainingCollectables)
            {
                newCollectableUI = Instantiate(collectedSlotPrefab, collectableUI.transform);
            }
            else
            {
                newCollectableUI = Instantiate(emptySlotPrefab, collectableUI.transform);
            }

            newCollectableUI.name = COLLECTABLE_SLOT_ID;
        }
    }
}
