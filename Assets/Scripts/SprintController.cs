using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SprintController : MonoBehaviour
{
    //========================================================================
    public float sprintMultiplier = 2f;
    public float originalSpeed;
    public MovementController movementController;
    //========================================================================

    //========================================================================
    void Start()
    {
        if (movementController != null) 
        {
            originalSpeed = movementController.moveSpeed;
        }
    }

    //========================================================================
    public void SetSprinting(bool isSprinting) 
    {
        if (movementController != null) 
        {
            movementController.moveSpeed = isSprinting ? originalSpeed * sprintMultiplier : originalSpeed;
        }
    }
}
