using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : InputBehaviour
{
    //TODO: change the way this class works. it should show UI when you first start pressing till you let go, and should proc a reset after holding

    private bool hasStartedReset = false;

    [SerializeField]
    private float yLimit = -50;



    void Start()
    {
        Subscribe(InputEvent.onResetLevel, ResetLevel);
    }

    private void FixedUpdate()
    {
        if (transform.position.y < yLimit)
        {
            ResetLevel();
        }
    }



    public void ResetLevel()
    {
        if (hasStartedReset) { return; }
        hasStartedReset = true;

        var player = GameObject.Find("Player");
        var rb = player.GetComponent<Rigidbody>();
        rb.velocity = rb.velocity.normalized * 0.25f;

        Camera.main.gameObject.transform.parent = null;

        AudioManager.Instance.PlaySFX(AudioID.Death);

        GameManager.TransitionToScene(SceneManager.GetActiveScene().name);
    }
}
