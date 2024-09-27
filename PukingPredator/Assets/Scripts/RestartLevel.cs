using UnityEngine.SceneManagement;

public class RestartLevel : InputBehaviour
{
    //TODO: change the way this class works. it should show UI when you first start pressing till you let go, and should proc a reset after holding

    void Start()
    {
        Subscribe(EventType.onResetLevel, GameInput_ResetLevel);
    }



    public void GameInput_ResetLevel()
    {
        ResetLevel();
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
