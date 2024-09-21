using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleController : MonoBehaviour
{
    //========================================================================
    public PlayerAnimator playerAnimator;
    public MovementController movementController;
    public Rigidbody2D rigidBody;
    public bool isGrappling;
    //========================================================================
    private float originalGravityScale;
    //========================================================================

    void Start()
    {
        originalGravityScale = rigidBody.gravityScale;
    }

    //========================================================================
    void Update()
    {
        if(IsContactingWall())
        {
            if (!isGrappling)
            {
                rigidBody.gravityScale = 0;
                rigidBody.velocity = Vector2.zero;
                isGrappling = true;
            }
        }
        else 
        {
            rigidBody.gravityScale = originalGravityScale;
        }
    }

    //========================================================================
    bool IsContactingWall()
    {
        if (movementController.movementInput == 0)
            return false;

        Vector2 direction = movementController.movementInput > 0 ? Vector2.right : Vector2.left;
        Vector2 rayStart = (Vector2)transform.position; 
        float wallCheckDistance = 0.25f;
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, wallCheckDistance, ~LayerMask.GetMask("Player"));
        return hit.collider != null && hit.collider.CompareTag("Terrain");
    }    

    //========================================================================
    public void Climb()
    {
        playerAnimator.ManuallyCycleSprite();
        rigidBody.AddForce(new Vector2(rigidBody.velocity.x, 1) * 400f, ForceMode2D.Impulse);
    }
    
    //========================================================================
    public void HoldPosition()
    {
        isGrappling = false;
    }
}
