using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : InputBehaviour
{
    //TODO: change the way this class works. it should show UI when you first start pressing till you let go, and should proc a reset after holding

    [SerializeField]
    private float yLimit = -50;

    void Start()
    {
        Subscribe(InputEvent.onResetLevel, GameInput_ResetLevel);
    }

    private void FixedUpdate()
    {
        if (transform.position.y < yLimit)
        {
            ResetLevel();
        }
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
