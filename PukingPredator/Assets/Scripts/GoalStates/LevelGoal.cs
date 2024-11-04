using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    [SerializeField] GameObject goalClearedText;

    /// <summary>
    /// Int to denote the level eg. tutorial: 0, level 1: 1 etc.
    /// </summary>
    [SerializeField] int levelIDInt;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.SetLevelCompleted(levelIDInt);
            goalClearedText.SetActive(true);
            Destroy(gameObject);
        }
    }

}
