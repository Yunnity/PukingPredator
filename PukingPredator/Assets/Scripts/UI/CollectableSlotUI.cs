using System.Collections;
using UnityEngine;

public class CollectableSlotUI : MonoBehaviour
{
    /// <summary>
    /// The collected version of the slot.
    /// </summary>
    [SerializeField]
    private GameObject collectedVersion;

    /// <summary>
    /// Lerp speed (with delta time) for how fast it goes to the slot.
    /// </summary>
    private float collectionRate = 4f;

    /// <summary>
    /// A multiplier to make the X lerp go faster.
    /// </summary>
    private float collectionRateXMult = 1.6f;



    IEnumerator GoToSlot()
    {
        var slotRect = collectedVersion.GetComponent<RectTransform>();

        Vector3 targetLocalPosition = Vector3.zero;
        while (Vector3.Distance(slotRect.localPosition, targetLocalPosition) > 1)
        {
            var rate = collectionRate * Time.deltaTime;
            var pos = slotRect.localPosition;
            pos.x = Mathf.Lerp(pos.x, targetLocalPosition.x, rate * collectionRateXMult);
            pos.y = Mathf.Lerp(pos.y, targetLocalPosition.y, rate);
            slotRect.localPosition = pos;
            yield return null;
        }
        slotRect.localPosition = targetLocalPosition;
    }

    public void TriggerCollected()
    {
        collectedVersion.SetActive(true);

        //set to the center of the screen
        var center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        collectedVersion.transform.position = center;

        StartCoroutine(GoToSlot());
    }

    /// <summary>
    /// Shows the collectable as collected without the animation
    /// Used when respawning on checlpoints
    /// </summary>
    public void DisplayCollectable()
    {
        collectedVersion.SetActive(true);
        var slotRect = collectedVersion.GetComponent<RectTransform>();
        
        slotRect.localPosition = Vector3.zero;
    }
}
