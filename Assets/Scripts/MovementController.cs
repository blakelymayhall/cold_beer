using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    //========================================================================
    public float moveSpeed;
    public float airMoveSpeed;
    public float sprintMultiplier;
    public bool isSprinting = false;
    public Rigidbody2D rigidBody;
    public float movementInput;
    public PlayerInputManager playerInputManager;
    //========================================================================
    private JumpController jumpController;
    private float originalSpeed;
    private bool sprintingInputPressed;

    //========================================================================
    void Start()
    {
        jumpController = playerInputManager.jumpController;
        originalSpeed = moveSpeed;
    }

    //========================================================================
    private void FixedUpdate() 
    {
        // this isn't right. 
        // if jump while sprinting, keep the sprint speed
        // if sprint while jumping, wait to hit ground before sprinting
        if(jumpController.jumpControllerState == JumpControllerState.Walking_Running)
        {
            isSprinting = sprintingInputPressed;
            moveSpeed = isSprinting ? originalSpeed * sprintMultiplier : originalSpeed;
        }
        else 
        {
            isSprinting = false;
        }

        if (jumpController.jumpControllerState != JumpControllerState.Walking_Running)
        {
            moveSpeed = airMoveSpeed;
        }
        rigidBody.velocity = new Vector2(movementInput * moveSpeed, rigidBody.velocity.y);
    }

    //========================================================================
    public void SetMovement(float movement) 
    {
        movementInput = movement;
    }

    //========================================================================
    public void SetSprintingInput(bool inputSprintActionPressed) 
    {
        sprintingInputPressed = inputSprintActionPressed;
    }
    
}
