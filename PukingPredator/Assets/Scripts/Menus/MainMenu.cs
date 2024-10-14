using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
        GameManager.TransitionToScene("PlayableLevel");
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
