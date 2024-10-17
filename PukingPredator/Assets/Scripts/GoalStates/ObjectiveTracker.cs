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

    void Start()
    {
        objectsLeftObject = GameObject.Find("objectsLeftText");
        objectsLeftText = objectsLeftObject.GetComponent<Text>();

        // find all objectives
        Objective[] objectives = FindObjectsOfType<Objective>();
        foreach(Objective obj in objectives)
        {
            obj.registerObjectiveTracker(this);
        }
        setRemainingText();
    }

    public void addCollectionObject()
    {
        remainingCollectionObjects++;
        setRemainingText();
    }

    public void removeCollectionObject()
    {
        remainingCollectionObjects--;
        setRemainingText();
    }

    private void setRemainingText()
    {
        objectsLeftText.text = "Remaining Collectables: " + remainingCollectionObjects;

        if (remainingCollectionObjects <= 0)
        {
            objectsLeftText.text = "Collected Everything";
        }
    }

}
