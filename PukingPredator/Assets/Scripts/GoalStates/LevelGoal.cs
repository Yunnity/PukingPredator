using System.Collections;
using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    [SerializeField]
    GameObject goalClearedText;

    private bool isReached = false;

    /// <summary>
    /// Delay to go to the next level in seconds.
    /// </summary>
    private float nextLevelDelay = 2f;



    void OnTriggerEnter(Collider other)
    {
        if (isReached) { return; }

        if (other.gameObject.tag == GameTag.player)
        {
            isReached = true;
            goalClearedText.SetActive(true);

            StartCoroutine(GoToNextLevel());
        }
    }

    private IEnumerator GoToNextLevel()
    {
        yield return new WaitForSeconds(nextLevelDelay);

        var collectedCount = FindObjectOfType<CollectableTracker>().collectedCount;
        GameManager.SetLevelCompleted(new(collectableCount: collectedCount));

        GameManager.TransitionToNextLevel();
    }
}
