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

    //========================================================================
    void Awake()
    {
        inputActions = new InputActions();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.performed += OnJump;
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
    void Start()
    {
        
    }

    //========================================================================
    void Update()
    {
        
    }

    //========================================================================
    private void OnMove(InputAction.CallbackContext context) 
    {
        if (movementController != null) 
        {
            movementController.SetMovement(context.ReadValue<float>());
        }
    }

    //========================================================================
    private void OnJump(InputAction.CallbackContext context) 
    {
        if (jumpController != null) 
        {
            jumpController.Jump();
        }
    }

    //========================================================================
    private void OnSprint(InputAction.CallbackContext context) 
    {
        bool isSprinting = context.ReadValueAsButton();
        if (sprintController != null) 
        {
            sprintController.SetSprinting(isSprinting);
        }
    }
}
