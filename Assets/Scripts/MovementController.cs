using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    //========================================================================
    public float baseSpeed;
    public float targetSpeed;
    public float sprintMultiplier;
    public float airSpeedMultiplier;
    public bool isSprinting = false;
    public Rigidbody2D rigidBody;
    public float movementInput;
    public PlayerInputManager playerInputManager;
    //========================================================================
    private JumpController jumpController;
    private float currentSpeed;
    private bool sprintingInputPressed;
    private float baseAcceleration = 15f;
    //========================================================================
    void Start()
    {
        jumpController = playerInputManager.jumpController;
    }

    //========================================================================
    private void FixedUpdate() 
    {
        // Init sprinting if on ground OR latch sprinting if in air 
        isSprinting = sprintingInputPressed && jumpController.jumpControllerState == JumpControllerState.Walking_Running ||
            isSprinting && jumpController.jumpControllerState == JumpControllerState.Airborne;
        
        // Set target speed
        float targetSpeed = 0;
        if(Mathf.Abs(movementInput) > 0)
        {
            if(jumpController.jumpControllerState == JumpControllerState.Walking_Running)
            {
                targetSpeed = isSprinting ? baseSpeed * sprintMultiplier : baseSpeed;
            }
            else
            {
                targetSpeed = 0.7f * (isSprinting ? baseSpeed * sprintMultiplier : baseSpeed);
            }
        }

        // Accelerate up to target speed
        if (Mathf.Abs(targetSpeed) > 0)  
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, baseAcceleration * Time.fixedDeltaTime);
        }
        else 
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, -baseAcceleration * Time.fixedDeltaTime);
        }
        currentSpeed = Mathf.Clamp(currentSpeed, -targetSpeed, targetSpeed);

        // Set rigid body motion
        rigidBody.velocity = new Vector2(movementInput * currentSpeed, rigidBody.velocity.y);
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
