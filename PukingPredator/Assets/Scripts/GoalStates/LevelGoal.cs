using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    [SerializeField] GameObject goalClearedText;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            goalClearedText.SetActive(true);
            Destroy(gameObject);
        }
    }
}
