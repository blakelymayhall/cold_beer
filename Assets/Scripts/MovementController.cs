using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    //========================================================================
    public float moveSpeed;
    public Rigidbody2D rigidBody;
    public float movementInput;
    //========================================================================
    

    //========================================================================
    void Start()
    {
        
    }

    //========================================================================
    private void FixedUpdate() 
    {
        rigidBody.velocity = new Vector2(movementInput * moveSpeed, rigidBody.velocity.y);
    }

    //========================================================================
    public void SetMovement(float movement) 
    {
        movementInput = movement;
    }
    
}
