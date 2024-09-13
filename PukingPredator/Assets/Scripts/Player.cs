using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    const float SPEED = 20;

    private void Start()
    {
        gameInput.onInteractAction += GameInput_OnInteract;
    }

    private void GameInput_OnInteract(object sender, System.EventArgs e)
    {
        // PRIMARY ACTION CODE
        Debug.Log("primary");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = gameInput.GetInputVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        transform.position += moveDir * SPEED * Time.deltaTime;
    }
}
