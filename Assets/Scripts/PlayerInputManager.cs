using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    //========================================================================
    public InputActions inputActions;
    public MovementController movementController;
    public JumpController jumpController;
    public SprintController sprintController;
    public GrappleController grappleController;
    //========================================================================

    void Awake()
    {
        inputActions = new InputActions();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.started += OnJump;
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
    }
    
    //========================================================================
    private void OnEnable() 
    {
        inputActions.Enable();
    }

    //========================================================================
    private void OnDisable() 
    {
        inputActions.Disable();
    }

    //========================================================================
    private void OnMove(InputAction.CallbackContext context) 
    {
        movementController.SetMovement(context.ReadValue<float>());
    }

    //========================================================================
    private void OnJump(InputAction.CallbackContext context) 
    {
        jumpController.Jump();
    }

    //========================================================================
    private void OnSprint(InputAction.CallbackContext context) 
    {
        bool sprintingInputPressed = context.ReadValueAsButton();
        sprintController.SetSprintingInput(sprintingInputPressed);
    }
}
