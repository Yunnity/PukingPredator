using UnityEngine;

public class MainMenu : MenuRoot
{
    [SerializeField]
    private GameObject settingsMenu;

    public void OnPlayButtonClicked()
    {
        GameManager.TransitionToScene("LevelSelect");
    }

    public void OnSettingsButtonClicked()
    {
        settingsMenu.SetActive(true);
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
