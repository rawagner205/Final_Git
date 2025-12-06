using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement
{
    public Animator walk;
    public float walkSpeed;
    public float jumpHeight;
    float movement = 0f;

    public PlayerMovement(Animator newAnim, float speed, float height)
    {
        walk = newAnim;
        walkSpeed = speed;
        jumpHeight = height;
    }

    public void MovePlayer(PlayerController player)
    {
        //move character forward
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            movement += .1f;
            walk.enabled = true;
            walk.Play(Animator.StringToHash("WalkCycle"));
        }
        //move character backward
        else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            movement -= .1f;
            walk.enabled = true;
            walk.Play(Animator.StringToHash("WalkCycleBackwards"));
        }
        else
        {
            movement = 0f;
            walk.enabled = false;
        }

        //apply movement 
        float moveAmount = movement * walkSpeed * Time.deltaTime;
        player.transform.Translate(moveAmount, 0, 0);
    }

    
    public void Jump(Rigidbody2D rb, TrackingBool canTallJump)
    {
        //increase jump height if canTallJump is activated 
        if (canTallJump.trackedBool == true)
        {
            rb.AddForce(new Vector2 (0, jumpHeight * 1.2f), ForceMode2D.Impulse);
        }
        //regular jump height if canTallJump isn't activated
        else
        {
            rb.AddForce(new Vector2 (0, jumpHeight), ForceMode2D.Impulse);
        }
    }
}
