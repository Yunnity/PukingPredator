using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField]
    private int levelNumber;

    public void OnClick()
    {
        GameManager.TransitionToLevel(levelNumber);
    }
}

