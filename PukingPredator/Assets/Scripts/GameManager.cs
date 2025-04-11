using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    private Dictionary<string, LevelCompletionData> completionData = new();
    public List<LevelCompletionData> completionDataList => levelIds.Select(l => completionData[l]).ToList();

    /// <summary>
    /// The name of the current level in unity.
    /// </summary>
    public string currentLevelId;

    private static bool _isGamePaused = false;
    public static bool isGamePaused {
        get
        {
            return _isGamePaused;
        }
        set
        {
            _isGamePaused = value;
            Cursor.lockState = _isGamePaused ? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }

    /// <summary>
    /// The names of levels in the unity editor.
    /// </summary>
    public List<string> levelIds = new();

    /// <summary>
    /// The scene the game will start on.
    /// </summary>
    [SerializeField]
    private string sceneToLoad = "";


    void Start()
    {
        if (sceneToLoad != "") { TransitionToScene(sceneToLoad); }

        foreach (var levelId in levelIds)
        {
            //TODO: load from persistent storage based on level name
            completionData[levelId] = new(collectableCount: 0, isDone: false);
        }
    }



    public static void SetLevelCompleted(LevelCompletionData data)
    {
        var currentData = Instance.completionData[Instance.currentLevelId];

        currentData.isDone = true;
        currentData.collectableCount = Mathf.Max(currentData.collectableCount, data.collectableCount);

        //TODO: save completion data to persistent storage
    }

    public static void TransitionToNextLevel()
    {
        var nextLevelNumber = Instance.levelIds.IndexOf(Instance.currentLevelId) + 1;
        if (nextLevelNumber >= Instance.levelIds.Count) { return; }
        TransitionToLevel(nextLevelNumber);
    }

    public static void TransitionToLevel(int levelNumber)
    {
        TransitionToScene(Instance.levelIds[levelNumber]);
    }

    public static void TransitionToScene(string sceneName)
    {
        var currentSceneName = SceneManager.GetActiveScene().name;

        bool currentIsLevel = Instance.levelIds.Contains(currentSceneName);
        bool targetIsLevel = Instance.levelIds.Contains(sceneName);

        if (!targetIsLevel) { Cursor.lockState = CursorLockMode.Confined; }
        if (targetIsLevel) { Instance.currentLevelId = sceneName; }

        if (targetIsLevel != currentIsLevel || currentSceneName == "Startup")
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayBackground(targetIsLevel ? MusicID.Game : MusicID.Title);
        }

        SceneTransition.Instance.FadeToScene(sceneName);
    }
}
