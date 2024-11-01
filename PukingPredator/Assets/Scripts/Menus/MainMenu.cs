using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuFirst;
    [SerializeField] private GameObject settingsMenuFirst;
    [SerializeField] private GameObject settingsMenu;
    public void OnPlayButtonClicked()
    {
        GameManager.TransitionToScene("PlayableLevelRemake");
    }

    public void OnSettingsButtonClicked()
    {
        settingsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settingsMenuFirst);
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    public void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }
}
