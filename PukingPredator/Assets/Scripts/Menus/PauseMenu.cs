using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : InputBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseMenuFirst;
    [SerializeField] private GameObject sensitivityMenu;

    void Start()
    {
        Subscribe(InputEvent.onPause, GameInput_Pause);
    }

    public void GameInput_Pause()
    {
        if (GameManager.isGamePaused) 
        {
            if (sensitivityMenu.activeSelf)
            {
                CloseSensitivityMenu();
            }
            else
            {
                Resume();
            }
        }
        else { Pause(); }
    }

    private void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameManager.isGamePaused = false;
    }

    private void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameManager.isGamePaused = true;
    }

    private void CloseSensitivityMenu()
    {
        sensitivityMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
    }

    public void OnResumeButtonClicked()
    {
        Resume();
    }

    public void OnSettingsButtonClicked()
    {
        if (!sensitivityMenu.activeSelf)
        {
            sensitivityMenu.SetActive(true);
        }
        else
        {
            sensitivityMenu.SetActive(false);
        }
    }

    public void OnBackButtonClicked()
    {
        CloseSensitivityMenu();
    }

    public void OnQuitButtonClicked()
    {
        Resume();
        GameManager.TransitionToScene("StartScreen");
    }

    public void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
    }
}
