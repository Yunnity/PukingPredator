using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : InputBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseMenuFirst;

    void Start()
    {
        Subscribe(InputEvent.onPause, GameInput_Pause);
    }

    public void GameInput_Pause()
    {
        PauseOrResume();
    }

    public void PauseOrResume()
    {
        if (GameManager.isGamePaused) Resume();
        else Pause();
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

    public void OnResumeButtonClicked()
    {
        Resume();
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
