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
    public float jumpForce = 300f;
    public PlayerInputManager inputManager;
    //========================================================================
    private float originalGravityScale;
    private bool performStep = false;
    private MovementController movementController;
    private JumpController jumpController;
    private float timer = 0f;
    private float wallDirection;
    private const float beginFallTime = 0.45f;
    //========================================================================

    void Start()
    {
        movementController = inputManager.movementController;
        jumpController = inputManager.jumpController;
        originalGravityScale = rigidBody.gravityScale;
    }

    //========================================================================
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (!IsContactingWall())
        {
            ResetGrapple();
            return;
        }

        if(!isGrappling && IsContactingWall())
        {
            InitializeGrapple();
        }

        if(isGrappling)
        {
            // Start falling if holding for too long
            StartFalling();

            // In grappling mode, each Jump-button strike will move the player up the wall
            ClimbStep();
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
        playerAnimator.SetJumpButtonPressed();
        performStep = true;
    }

    //========================================================================
    void ResetGrapple()
    {
        isGrappling = false;
        rigidBody.gravityScale = originalGravityScale;
        if (ShouldTurnOffWallJump())
        {
            jumpController.SetCanJumpOffWall(false); 
        }        
    }
    
    //========================================================================
    void InitializeGrapple()
    {
        // Start grappling mode
        wallDirection = movementController.movementInput;
        rigidBody.gravityScale = 0;
        rigidBody.velocity = Vector2.zero;
        isGrappling = true;
        jumpController.SetCanJumpOffWall(true);
        timer = 0;
    }
    
    //========================================================================
    void StartFalling()
    {
        if (timer > beginFallTime)
        {
            rigidBody.gravityScale = originalGravityScale / 3;
        }
    }

    //========================================================================
    void ClimbStep()
    {
        if(performStep)
        {
            if(IsHeadAboveWall())
            {
                // Jump over remainder of the wall
                rigidBody.AddForce(new Vector2(rigidBody.velocity.x, 1) * jumpForce, ForceMode2D.Impulse);
                rigidBody.gravityScale = originalGravityScale;
            }
            else 
            {
                // Step up on the wall by discrete amount
                Vector2 newPosition = rigidBody.position + new Vector2(0, grappleJumpDistance * Time.fixedDeltaTime);
                rigidBody.MovePosition(newPosition);
                timer = 0;
                rigidBody.gravityScale = 0;
                rigidBody.velocity = Vector2.zero;
            }   

            performStep = false;
        }
    }

    //========================================================================
    bool IsHeadAboveWall()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        Vector2 direction = movementController.movementInput > 0 ? Vector2.right : Vector2.left;
        Vector2 rayStart = (Vector2)transform.position + new Vector2(0, playerCollider.bounds.extents.y); 
        float wallCheckDistance = 0.25f;
        Debug.DrawRay(rayStart, direction * wallCheckDistance, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, wallCheckDistance, ~LayerMask.GetMask("Player"));
        return hit.collider == null || !hit.collider.CompareTag("Terrain");
    }

    //========================================================================
    bool ShouldTurnOffWallJump()
    {
        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection = wallDirection > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 10f, ~LayerMask.GetMask("Player"));
        
        if (hit.collider != null)
        {
            if (hit.distance < 0.5 && hit.collider.CompareTag("Terrain"))
            {
                return false;
            }
        }

        return true;
    }
}
