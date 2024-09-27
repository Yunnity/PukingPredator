using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    /// <summary>
    /// The scene the game will start on.
    /// </summary>
    [SerializeField]
    private string sceneToLoad;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
