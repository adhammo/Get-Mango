using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_movement : MonoBehaviour
{
    public CharacterController2D Contoller;
    public PlayerCharacter character;
    bool jump = false;
    public Animator animator;
    // Update is called once per frame
    void Update()
    {

        character.SetState(Input.GetAxisRaw("Horizontal"), jump);
        animator.SetFloat("speed", Mathf.Abs(Input.GetAxisRaw("Horizontal")));
        if (Input.GetKey("up")|| Input.GetAxisRaw("Jump")==1|| Input.GetAxisRaw("Vertical") == 1)
        {
            jump = true;
        }
        else jump = false;
        animator.SetBool("isjumping", jump);
        animator.SetFloat("jumping", Contoller.Velocity.y);
    }
}
