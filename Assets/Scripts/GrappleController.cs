using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleController : MonoBehaviour
{
    //========================================================================
    public PlayerAnimator playerAnimator;
    public MovementController movementController;
    public JumpController jumpController;
    public Rigidbody2D rigidBody;
    public bool isGrappling = false;
    //========================================================================
    private float originalGravityScale;
    private bool performStep = false;
    //========================================================================

    void Start()
    {
        originalGravityScale = rigidBody.gravityScale;
    }

    //========================================================================
    void Update()
    {
        if (!IsContactingWall())
        {
            Reset();
            return;
        }

        if(!isGrappling)
        {
            rigidBody.gravityScale = 0;
            rigidBody.velocity = Vector2.zero;
            isGrappling = true;
        }

        if(isGrappling && performStep)
        {
            Collider2D playerCollider = GetComponent<Collider2D>();
            Vector2 direction = movementController.movementInput > 0 ? Vector2.right : Vector2.left;
            Vector2 rayStart = (Vector2)transform.position + new Vector2(0, playerCollider.bounds.extents.y); 
            float wallCheckDistance = 0.25f;
            Debug.DrawRay(rayStart, direction * wallCheckDistance, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, wallCheckDistance, ~LayerMask.GetMask("Player"));
            if(hit.collider == null || !hit.collider.CompareTag("Terrain"))
            {
                jumpController.Jump(true);
            }
            else 
            {
                Vector2 newPosition = rigidBody.position + new Vector2(0, 15f * Time.fixedDeltaTime);
                rigidBody.MovePosition(newPosition);
            }   
            performStep = false;
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
        if (isGrappling)
        {
            playerAnimator.SetJumpButtonPressed();
            performStep = true;
        }
    }
    
    //========================================================================
    public void Reset()
    {
        isGrappling = false;
        rigidBody.gravityScale = originalGravityScale;
    }
}
