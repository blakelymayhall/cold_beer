using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class JumpController : MonoBehaviour
{
    //========================================================================
    public float normalJumpForce;
    public float sprintJumpForce;
    public float climbSpeed;
    public float jumpAscentGravityScale;
    public float jumpDescentGravityScale;
    public Rigidbody2D rigidBody;
    public PlayerInputManager inputManager;
    public JumpControllerState jumpControllerState;
    public PlayerAnimator playerAnimator;
    //========================================================================
    private MovementController movementController;
    public bool isClimbing = false;
    private float originalGravityScale;
    private float wallDirection;
    private Vector3 climbStepTarget;
    private float fallTimer;
    private float jumpTimer;
    private float timeLastJump;

    private const float beginFallTime_sec = 0.45f;
    private const float allowWallJumpTimeThreshold_sec = 0.15f;
    private const float climbDistance = 50f;

    //========================================================================
    void Start()
    {
        movementController = inputManager.movementController;
        jumpControllerState = JumpControllerState.Walking_Running;
        originalGravityScale = rigidBody.gravityScale;
    }
    
    //========================================================================  
    void FixedUpdate()
    {
        fallTimer += Time.fixedDeltaTime;
        jumpTimer += Time.fixedDeltaTime;

        jumpControllerState = SetJumpState();
        SetAncillariesBasedOnState();
        
    }

    //========================================================================
    public JumpControllerState SetJumpState()
    {
        if(PlayerTouchingGround())
        {
            return JumpControllerState.Walking_Running;
        }

        if(jumpControllerState == JumpControllerState.Climbing_Step)
        {
            if (IsHeadAboveWall())
            {
                return JumpControllerState.Climbing_OverLedge;
            }

            if(!IsContactingWall())
            {
                return JumpControllerState.WallJump;
            }
            
            if(Vector2.Distance(rigidBody.position, climbStepTarget) < 0.01)
            {
                // completed climb step; re-init climbing
                return JumpControllerState.Init_Climbing;
            }

            return JumpControllerState.Climbing_Step;
        }

        if(IsContactingWall())
        {
            if (isClimbing)
            {
                if (fallTimer > beginFallTime_sec)
                {
                    return JumpControllerState.Climbing_Falling;
                }
                if (IsHeadAboveWall())
                {
                    return JumpControllerState.Climbing_OverLedge;
                }
                return JumpControllerState.Climbing;
            }
            else 
            {
                return JumpControllerState.Init_Climbing;
            }
        }

        return JumpControllerState.Airborne;
    } 

    void SetAncillariesBasedOnState()
    {
        switch (jumpControllerState)
        {
            case JumpControllerState.Airborne:
            {
                isClimbing = false;
                if (rigidBody.velocity.y > 0)
                {
                    rigidBody.gravityScale = originalGravityScale*jumpAscentGravityScale;
                }
                else 
                {
                    rigidBody.gravityScale = originalGravityScale*jumpDescentGravityScale;
                }
                
                break;
            }   
            case JumpControllerState.Walking_Running:
            {
                isClimbing = false;
                rigidBody.gravityScale = originalGravityScale;
                break;
            }
            case JumpControllerState.Init_Climbing:
            {
                fallTimer = 0;
                isClimbing = true;
                rigidBody.gravityScale = 0;
                rigidBody.velocity = Vector2.zero;
                wallDirection = movementController.movementInput;
                break;
            }
            case JumpControllerState.Climbing_Step:
            {
                rigidBody.position = Vector3.Lerp(rigidBody.position, 
                    climbStepTarget, 
                    Time.fixedDeltaTime * climbSpeed);
                break;
            }
            case JumpControllerState.Climbing_Falling:
            {
                rigidBody.gravityScale = originalGravityScale / 3;
                break;
            }
            case JumpControllerState.WallJump:
            {
                if (jumpTimer - timeLastJump < allowWallJumpTimeThreshold_sec)
                {
                    float jumpForce = movementController.isSprinting ? sprintJumpForce : normalJumpForce;
                    rigidBody.AddForce(new Vector2(0, 1) * jumpForce, ForceMode2D.Impulse);
                    jumpTimer = 0;
                    timeLastJump = 999f;
                }
                jumpControllerState = JumpControllerState.Airborne;
                goto case JumpControllerState.Airborne;
            }
        }
    }

    //========================================================================
    public void Jump() 
    {        
        timeLastJump = jumpTimer;
        switch (jumpControllerState)
        {
            case JumpControllerState.Walking_Running:
            {
                float jumpForce = movementController.isSprinting ? sprintJumpForce : normalJumpForce;
                rigidBody.AddForce(new Vector2(0, 1) * jumpForce, ForceMode2D.Impulse);
                break;
            }
            case JumpControllerState.Climbing_Falling:
            {
                // Go back to climbing if jump pressed while falling down wall
                rigidBody.gravityScale = 0;
                rigidBody.velocity = Vector2.zero;
                goto case JumpControllerState.Climbing;
            }
            case JumpControllerState.Climbing: 
            {
                // Initiate climb step if jump pressed while on wall
                climbStepTarget = rigidBody.position + new Vector2(0, climbDistance * Time.fixedDeltaTime);
                jumpControllerState = JumpControllerState.Climbing_Step;
                playerAnimator.SetJumpButtonPressed();
                break;
            }
            case JumpControllerState.Climbing_OverLedge:
            {
                // Jump over the edge freely if jump pressed while head is over wall
                jumpControllerState = JumpControllerState.Airborne;
                goto case JumpControllerState.Walking_Running;
            }
        }
    }

    //======================================================================== 
    bool PlayerTouchingGround()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        Vector2 rayStart = (Vector2)transform.position - new Vector2(0, playerCollider.bounds.extents.y); 
        Vector2 rayDirection = Vector2.down; 
        float rayLength = 0.1f;  
        RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, rayLength, ~LayerMask.GetMask("Player"));
        return hit.collider != null && hit.collider.CompareTag("Terrain");
    }

    //========================================================================
    bool IsContactingWall()
    {
        if (movementController.movementInput == 0)
            return false;
        
        Vector2 direction = movementController.movementInput > 0 ? Vector2.right : Vector2.left;
        Vector2 rayStart = (Vector2)transform.position; 
        float wallCheckDistance = 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, wallCheckDistance, ~LayerMask.GetMask("Player"));
        return hit.collider != null && hit.collider.CompareTag("Terrain") && !IsWallShort(hit);
    }    

    //========================================================================
    bool IsWallShort(RaycastHit2D hit)
    {
        Tilemap hitTilemap = hit.collider.GetComponent<Tilemap>();

        if (hitTilemap != null)
        {
            Vector3 hitPoint = hit.point;
            Vector3Int tilePosition = hitTilemap.WorldToCell(hitPoint);

            // check left tile and right tiles to find adjacent 'ground'
            // if ground within one unit, then the tile is too short to init climb
            Vector3Int down = new Vector3Int(0, -1, 0); 
            Vector3Int left = new Vector3Int(-1, 0, 0); 
            Vector3Int right = new Vector3Int(1, 0, 0);

            // left
            TileBase tile = hitTilemap.GetTile(tilePosition + left);
            if(tile == null) // no tile to the left, check if ground is one down
            {  
                if(hitTilemap.GetTile(tilePosition + left + down) != null)
                {
                    return true;
                }
            }

            // right
            tile = hitTilemap.GetTile(tilePosition + right);
            if(tile == null) // no tile to the right, check if ground is one down
            {  
                if(hitTilemap.GetTile(tilePosition + right + down) != null)
                {
                    return true;
                }
            }   
        }

        return false;
    }    

    //========================================================================
    bool IsHeadAboveWall()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        Vector2 direction = wallDirection > 0 ? Vector2.right : Vector2.left;
        Vector2 rayStart = (Vector2)transform.position + new Vector2(0, playerCollider.bounds.extents.y); 
        float wallCheckDistance = 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, wallCheckDistance, ~LayerMask.GetMask("Player"));
        return hit.collider == null || !hit.collider.CompareTag("Terrain");
    }
}

public enum JumpControllerState
{
    Walking_Running,
    Airborne,
    Init_Climbing,
    Climbing,
    Climbing_Step,
    Climbing_Falling,
    Climbing_OverLedge,
    Climbing_DirectionChange,
    WallJump
}
