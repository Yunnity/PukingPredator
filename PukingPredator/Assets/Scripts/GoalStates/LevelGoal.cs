using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    [SerializeField]
    GameObject goalClearedText;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == GameTag.player)
        {
            var collectedCount = FindObjectOfType<CollectableTracker>().collectedCount;

            GameManager.Instance.SetLevelCompleted(new(collectableCount: collectedCount));
            goalClearedText.SetActive(true);
            Destroy(gameObject);
        }
    }

}
