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
        isSprinting = sprintingInputPressed;
        float baseSpeed = isSprinting ? originalSpeed * sprintMultiplier : originalSpeed;
        
        if (jumpController.jumpControllerState == JumpControllerState.Walking_Running)
        {
            moveSpeed = baseSpeed;
        }
        else
        {
            moveSpeed = 0.7f * baseSpeed;
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
