using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    //========================================================================
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    //========================================================================
    private float movementInput;

    //========================================================================
    void Start()
    {
        
    }

    //========================================================================
    private void FixedUpdate() 
    {
        if (rb != null) 
        {
            rb.velocity = new Vector2(movementInput * moveSpeed, rb.velocity.y);
        }
    }

    //========================================================================
    public void SetMovement(float movement) 
    {
        movementInput = movement;
    }
    
}
