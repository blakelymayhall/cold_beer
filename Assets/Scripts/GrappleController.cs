using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleController : MonoBehaviour
{
    //========================================================================
    public PlayerAnimator playerAnimator;
    public Rigidbody2D rigidBody;
    public bool isGrappling = false;
    public float grappleJumpDistance = 15f;
    public PlayerInputManager inputManager;
    //========================================================================
    private float originalGravityScale;
    private bool performStep = false;
    private MovementController movementController;
    private JumpController jumpController;
    //========================================================================

    void Start()
    {
        movementController = inputManager.movementController;
        jumpController = inputManager.jumpController;
        originalGravityScale = rigidBody.gravityScale;
    }

    //========================================================================
    void Update()
    {
        if (!IsContactingWall())
        {
            isGrappling = false;
            rigidBody.gravityScale = originalGravityScale;
            return;
        }

        if(!isGrappling)
        {
            // Start grappling mode
            rigidBody.gravityScale = 0;
            rigidBody.velocity = Vector2.zero;
            isGrappling = true;
        }

        if(isGrappling && performStep)
        {
            // In grappling mode, each Jump-button strike will move the player up the wall
            if(IsHeadAboveWall())
            {
                jumpController.Jump(true);
            }
            else 
            {
                Vector2 newPosition = rigidBody.position + new Vector2(0, grappleJumpDistance * Time.fixedDeltaTime);
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
    private bool IsHeadAboveWall()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        Vector2 direction = movementController.movementInput > 0 ? Vector2.right : Vector2.left;
        Vector2 rayStart = (Vector2)transform.position + new Vector2(0, playerCollider.bounds.extents.y); 
        float wallCheckDistance = 0.25f;
        Debug.DrawRay(rayStart, direction * wallCheckDistance, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, wallCheckDistance, ~LayerMask.GetMask("Player"));
        return hit.collider == null || !hit.collider.CompareTag("Terrain");
    }
}
