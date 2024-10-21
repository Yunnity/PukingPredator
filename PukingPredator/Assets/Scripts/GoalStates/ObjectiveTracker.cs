using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveTracker : MonoBehaviour
{
    /*
     The player must collect all objectives to win

    Objectives get the "Objective" script, and this script will register all objectives upon loading.
    The objectives will call functions in this objectiveTracker when they are collected.
     */

    private GameObject objectsLeftObject;
    private Text objectsLeftText;

    private int remainingCollectionObjects;

    /// <summary>
    /// The prefab for the collectables UI
    /// </summary>
    [SerializeField]
    private GameObject collectablesUIPrefab;
    public GameObject emptySlotPrefab;
    public GameObject collectedSlotPrefab;

    /// <summary>
    /// The panel that the collectables are contained in.
    /// </summary>
    private GameObject UIPanel;

    /// <summary>
    /// The script associated with the Collectable UI
    /// </summary>
    private CollectablesUI collectablesUI;

    private int totalObjects;


    void Start()
    {
        var canvas = GameObject.Find("Canvas");
        UIPanel = Instantiate(collectablesUIPrefab, canvas.transform);
        collectablesUI = UIPanel.GetComponent<CollectablesUI>();

        //objectsLeftObject = GameObject.Find("objectsLeftText");
        //objectsLeftText = objectsLeftObject.GetComponent<Text>();

        // find all objectives
        Objective[] objectives = FindObjectsOfType<Objective>();
        foreach(Objective obj in objectives)
        {
            obj.registerObjectiveTracker(this);
        }
        
        totalObjects = remainingCollectionObjects;
        UpdateCollectablesUI();
    }

    public void addCollectionObject()
    {
        remainingCollectionObjects++;
        UpdateCollectablesUI();
    }

    public void removeCollectionObject()
    {
        remainingCollectionObjects--;
        UpdateCollectablesUI();
    }

    private void setRemainingText()
    {
        //objectsLeftText.text = "Remaining Collectables: " + remainingCollectionObjects;

        //if (remainingCollectionObjects <= 0)
        //{
        //    objectsLeftText.text = "Collected Everything";
        //}
    }

    public void UpdateCollectablesUI()
    {
        const string COLLECTABLE_SLOT_ID = "CollectableSlotUI";
        foreach (Transform child in UIPanel.transform)
        {
            if (child.gameObject.name == COLLECTABLE_SLOT_ID)
            {
                Destroy(child.gameObject);
            }
        }

        GameObject newCollectableUI;
        for (int i = 0; i < totalObjects; i++)
        {
            if (totalObjects - i > remainingCollectionObjects)
            {
                newCollectableUI = Instantiate(collectedSlotPrefab, UIPanel.transform);
            } else
            {
                newCollectableUI = Instantiate(emptySlotPrefab, UIPanel.transform);
            }

            newCollectableUI.name = COLLECTABLE_SLOT_ID;
        }
    }

}
