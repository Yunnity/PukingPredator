using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Note : MonoBehaviour
{
    /// <summary>
    /// The alpha of the note visuals.
    /// </summary>
    private float alpha = 0f;

    /// <summary>
    /// Delay in seconds before making the note appear. Intended for tutorials.
    /// </summary>
    [SerializeField]
    private float delay = 0f;

    /// <summary>
    /// The rate that the image fades in and out.
    /// </summary>
    private float fadeSpeed = 4f;

    /// <summary>
    /// If the player was nearby the last time a check was performed.
    /// </summary>
    private bool isPlayerNearby = false;



    private void Start()
    {
        SetAlpha(0f);

        //make sure the collider is set to trigger
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPlayerNearby) { return; }

        if (other.gameObject.CompareTag(GameTag.player))
        {
            isPlayerNearby = true;
            StopAllCoroutines(); // Stop any ongoing fading out
            StartCoroutine(FadeAlpha(1f)); // Fade in to alpha = 1
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isPlayerNearby) { return; }

        if (other.gameObject.CompareTag(GameTag.player))
        {
            isPlayerNearby = false;
            StopAllCoroutines(); // Stop any ongoing fading in
            StartCoroutine(FadeAlpha(0f)); // Fade out to alpha = 0
        }
    }



    /// <summary>
    /// Coroutine to fade the image to the target alpha over time.
    /// </summary>
    /// <param name="targetAlpha"></param>
    /// <returns></returns>
    IEnumerator FadeAlpha(float targetAlpha)
    {
        while (!Mathf.Approximately(alpha, targetAlpha))
        {
            // Handle the delay if trying to go up.
            if (targetAlpha > alpha)
            {
                while (delay > 0)
                {
                    delay -= Time.deltaTime;
                    yield return null;
                }
            }

            // Gradually adjust the alpha based on fadeSpeed and deltaTime
            alpha = Mathf.MoveTowards(alpha, targetAlpha, fadeSpeed * Time.deltaTime);

            // Set the new alpha value to the image
            SetAlpha(alpha);

            yield return null;
        }
    }

    /// <summary>
    /// Helper function to set the alpha of the visuals.
    /// </summary>
    /// <param name="alpha"></param>
    protected abstract void SetAlpha(float alpha);
}
