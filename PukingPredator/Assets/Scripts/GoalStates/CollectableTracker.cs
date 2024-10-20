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



    void Start()
    {
        collectableUI = GameObject.Find("objectsLeftText");
        collectablesLeftText = collectableUI.GetComponent<Text>();

        // find all collectables
        var collectables = FindObjectsOfType<Collectable>();
        remainingCollectables = collectables.Length;
        foreach (var collectable in collectables)
        {
            collectable.tracker = this;
        }

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

    private void UpdateUI()
    {
        if (remainingCollectables > 0)
        {
            collectablesLeftText.text = $"Remaining Collectables: {remainingCollectables}";
        }
        else
        {
            collectablesLeftText.text = "Collected Everything";
        }
    }

}
