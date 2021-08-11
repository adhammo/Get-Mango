using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController2D Controller;
    PlayerCharacter playercharacter;
    Animator animator;
    Rigidbody2D rb;

    bool jump = false;

    float horizontalMove = 0f;

   

    // Start is called before the first frame update
    void Start()
    {
        Controller = GetComponent<CharacterController2D>();
        playercharacter = GetComponent<PlayerCharacter>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        horizontalMove = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        animator.SetFloat("ySpeed", Controller.Velocity.y);

        if(Controller.Velocity.y < 0f)
        {
            animator.SetBool("isFalling", true);
        }

        if (Input.GetButton("Jump"))
        {
            jump = true;
            animator.SetBool("isJumping", true);
        }

        playercharacter.SetState(horizontalMove, jump);

        jump = false;
        
        if(Controller.IsGrounded == true)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }

        
    }
}
