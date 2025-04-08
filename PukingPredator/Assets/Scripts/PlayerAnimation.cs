using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private GameInput gameInput;

    private int isMovingHash = Animator.StringToHash("isMoving");
    private int isPukingHash = Animator.StringToHash("isPuking");
    private int isJumpingHash = Animator.StringToHash("isJumping");
    private int isEatingHash = Animator.StringToHash("isEating");

    [SerializeField]
    private GameObject model;

    private Rigidbody rb;



    void Start()
    {
        animator = model.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        gameInput = GameInput.Instance;
    }

    void Update()
    {
        UpdateMovementState();
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

    private void ResetTriggers()
    {
        animator.ResetTrigger(isPukingHash);
        animator.ResetTrigger(isEatingHash);
        animator.ResetTrigger(isJumpingHash);
    }

    public void StartEatAnim()
    {
        ResetTriggers();
        animator.SetTrigger(isEatingHash);
    }

    public void StartJumpAnim()
    {
        animator.SetTrigger(isJumpingHash);
    }

    public void StartPukeAnim()
    {
        ResetTriggers();
        animator.SetTrigger(isPukingHash);
    }

    private void UpdateMovementState()
    {
        bool isMoving = gameInput.movementInput.magnitude > 0;
        bool animatorIsMoving = animator.GetBool(isMovingHash);
        if (isMoving != animatorIsMoving)
        {
            animator.SetBool(isMovingHash, isMoving);
        }
    }
}
