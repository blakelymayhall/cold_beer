using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SprintController : MonoBehaviour
{
    //========================================================================
    public float sprintMultiplier;
    public float originalSpeed;
    public bool isSprinting = false;
    public PlayerInputManager inputManager;
    //========================================================================
    private MovementController movementController;
    private JumpController jumpController;
    private bool sprintingInputPressed = false;

    //========================================================================
    void Start()
    {
        jumpController = inputManager.jumpController;
        movementController = inputManager.movementController;
        originalSpeed = movementController.moveSpeed;
    }

    //========================================================================
    void FixedUpdate()
    {
        isSprinting = sprintingInputPressed && !jumpController.IsJumping();
        movementController.moveSpeed = isSprinting ? originalSpeed * sprintMultiplier : originalSpeed;
    }

    //========================================================================
    public void SetSprintingInput(bool inputSprintActionPressed) 
    {
        sprintingInputPressed = inputSprintActionPressed;
    }
}
