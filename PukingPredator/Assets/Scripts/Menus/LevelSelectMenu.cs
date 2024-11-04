using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectMenu : MonoBehaviour
{

    public static LevelSelectMenu instance { get; private set; }

    [SerializeField] private GameObject tutorialLevelButton;
    [SerializeField] private GameObject firstLevelButton;

    private void Start()
    {
        SetButtonsActive();
    }

    private void SetButtonsActive()
    {
        int maxLevelCompleted = GameManager.Instance.maxLevelCompleted;
        firstLevelButton.SetActive(maxLevelCompleted >= 1);
    }

    public void OnTutorialButtonClicked()
    {
        GameManager.TransitionToScene("TutorialLevel");
    }

    public void OnFirstLevelButtonclicked()
    {
        GameManager.TransitionToScene("PlayableLevelRemake");
    }

    public void OnBackButtonClicked()
    {
        GameManager.TransitionToScene("StartScreen");
    }

}
