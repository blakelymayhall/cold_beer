using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SprintController : MonoBehaviour
{
    //========================================================================
    public float sprintMultiplier;
    public float originalSpeed;
    public MovementController movementController;
    public JumpController jumpController;
    public bool isSprinting = false;
    //========================================================================
    private bool sprintingInputPressed = false;

    //========================================================================
    void Start()
    {
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
