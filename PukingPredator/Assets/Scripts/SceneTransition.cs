using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : SingletonMonobehaviour<SceneTransition>
{
    [SerializeField]
    private Image fadePrefab;

    private Image fadeImage;

    private string nextSceneName;

    public void FadeToScene(string sceneName)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        fadeImage = Instantiate(fadePrefab, canvas.transform);

        nextSceneName = sceneName;
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        float timeElapsed = 0f;
        float fadeDuration = 1f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;

            // Lerps alpha to transparent
            fadeImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration));

            if (timeElapsed < fadeDuration + fadeDuration / 2) SceneManager.LoadScene(nextSceneName);
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float timeElapsed = 0f;
        float fadeDuration = 1f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;

            // Lerps alpha to be fully opaque
            fadeImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration));
            yield return null;
        }

        yield return StartCoroutine(FadeOut());
    }
}
