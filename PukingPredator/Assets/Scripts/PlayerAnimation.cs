using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    private const float MOVING_THRESHOLD = 0.001f;

    [SerializeField]
    private GameObject model;

    private Animator animator;
    private Rigidbody rb;

    int isMovingHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = model.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        isMovingHash = Animator.StringToHash("isMoving");
        
    }

    // Update is called once per frame
    void Update()
    {
        updateStates();
    }

    private void PrintAnimatorState()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        if (clipInfo.Length > 0)
        {
            // Print the name of the animation clip currently playing
            Debug.Log("Current Animation State: " + clipInfo[0].clip.name);
        }
    }

    // Check animation state
    // Currently, we only have moving and idle
    private void updateStates()
    {
        // TODO WE NEED TO CHANGE THIS but we can do it when we have more animations
        bool horizontalMovement = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) > 0;
        bool isMoving = animator.GetBool(isMovingHash);
        if ((!isMoving && horizontalMovement) || (isMoving && !horizontalMovement))
        {
            animator.SetBool(isMovingHash, horizontalMovement);
        }
    }
}
