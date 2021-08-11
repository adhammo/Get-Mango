using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_movement : MonoBehaviour
{
    public CharacterController2D Contoller;
    public PlayerCharacter character;
    bool jump = false;

    // Update is called once per frame
    void Update()
    {
        character.SetState(Input.GetAxisRaw("Horizontal"), jump);

        if (Input.GetKey("up")|| Input.GetAxisRaw("Jump")==1|| Input.GetAxisRaw("Vertical") == 1)
        {
            jump = true;
        }
        else jump = false;
    }
}
