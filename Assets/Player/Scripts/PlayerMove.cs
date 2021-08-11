using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(PlayerCharacter))]
[RequireComponent(typeof(Animator))]
public class PlayerMove : MonoBehaviour
{
    readonly int hashHorizontalPara = Animator.StringToHash("Horizontal");
    readonly int hashAnyInputPara = Animator.StringToHash("AnyInput");
    readonly int hashVerticalPara = Animator.StringToHash("Vertical");
    readonly int hashGroundedPara = Animator.StringToHash("Grounded");

    CharacterController2D characterController;
    PlayerCharacter playerCharacter;
    Animator animator;

    void Awake()
    {
        characterController = GetComponent<CharacterController2D>();
        playerCharacter = GetComponent<PlayerCharacter>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        bool jump = Input.GetAxisRaw("Vertical") > 0f;
        playerCharacter.SetState(horizontal, jump, false);
        animator.SetBool(hashAnyInputPara, !Mathf.Approximately(horizontal, 0f));
    }

    void FixedUpdate()
    {
        animator.SetFloat(hashHorizontalPara, playerCharacter.Movement.x);
        animator.SetFloat(hashVerticalPara, playerCharacter.Movement.y);
        animator.SetBool(hashGroundedPara, characterController.IsGrounded);
    }
}
