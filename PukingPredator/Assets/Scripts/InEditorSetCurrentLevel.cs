using UnityEngine;
using UnityEngine.SceneManagement;

public class InEditorSetCurrentLevel : MonoBehaviour
{
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        GameManager.Instance.currentLevelId = currentScene.name;
    }
}

