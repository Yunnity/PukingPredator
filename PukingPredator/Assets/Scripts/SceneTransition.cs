using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : SingletonMonobehaviour<SceneTransition>
{
    [SerializeField]
    private Image fadePrefab;



    private Image CreateFadeImage()
    {
        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null) { return null; }
        return Instantiate(fadePrefab, canvas.transform);
    }

    public void FadeToScene(string sceneName)
    {
        //TODO: remove this once the menus are sorted out
        var currentSceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "StartScreen" && currentSceneName == "LevelSelect"
         || sceneName == "LevelSelect" && currentSceneName == "StartScreen")
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        StartCoroutine(FadeOut(sceneName));
    }

    /// <summary>
    /// Fades into the scene from the transition.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeIn()
    {
        Image fadeImage = CreateFadeImage();
        if (fadeImage == null) { yield break; }

        float timeElapsed = 0f;
        float fadeDuration = 1f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            
            // Lerps alpha to transparent
            fadeImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration));

            if (timeElapsed < fadeDuration + fadeDuration / 2) 
            yield return null;
        }

        Destroy(fadeImage.gameObject);
    }

    /// <summary>
    /// Fades out of the current scene into the transition.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator FadeOut(string sceneName)
    {
        Image fadeImage = CreateFadeImage();
        if (fadeImage != null)
        {
            float timeElapsed = 0f;
            float fadeDuration = 1f;

            while (timeElapsed < fadeDuration)
            {
                timeElapsed += Time.deltaTime;

                // Lerps alpha to be fully opaque
                fadeImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration));
                yield return null;
            }
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        // Wait for the scene to finish loading
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(FadeIn());
    }
}
