using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    /// <summary>
    /// The scene the game will start on.
    /// </summary>
    [SerializeField]
    private string sceneToLoad;

    public static bool isGamePaused;


    // Start is called before the first frame update
    void Start()
    {
        TransitionToScene(sceneToLoad);
    }



    public static void TransitionToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
