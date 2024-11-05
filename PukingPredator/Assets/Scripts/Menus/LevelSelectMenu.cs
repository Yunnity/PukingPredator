using System.Collections.Generic;
using UnityEngine;

public class LevelSelectMenu : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> levelButtons = new();

    private void Start()
    {
        SetButtonsActive();
    }



    public void OnBackButtonClicked()
    {
        GameManager.TransitionToScene("StartScreen");
    }

    private void SetButtonsActive()
    {
        var completionData = GameManager.Instance.completionDataList;
        var completedSoFar = true;
        for (var i = 0; i < completionData.Count; i++)
        {
            levelButtons[i].SetActive(completedSoFar);
            if (!completionData[i].isDone) { completedSoFar = false; }
        }
    }
}
