using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    //========================================================================
    public float normalJumpForce;
    public float sprintJumpForce;
    public float climbSpeed;
    public Rigidbody2D rigidBody;
    public PlayerInputManager inputManager;
    public JumpControllerState jumpControllerState;
    public PlayerAnimator playerAnimator;
    //========================================================================
    private SprintController sprintController;
    private MovementController movementController;
    public bool isClimbing = false;
    private float originalGravityScale;
    private float wallDirection;
    private Vector3 climbStepTarget;
    private float fallTimer;
    private float jumpTimer;
    private float timeLastJump;

    private const float beginFallTime = 0.45f;
    //========================================================================
    void Start()
    {
        sprintController = inputManager.sprintController;
        movementController = inputManager.movementController;
        jumpControllerState = JumpControllerState.Walking_Running;
        originalGravityScale = rigidBody.gravityScale;
    }

    void FixedUpdate()
    {
        fallTimer += Time.fixedDeltaTime;
        jumpTimer += Time.fixedDeltaTime;

        jumpControllerState = SetJumpState();
        switch (jumpControllerState)
        {
            case JumpControllerState.Airborne:
            {
                isClimbing = false;
                rigidBody.gravityScale = originalGravityScale;
                break;
            }   
            case JumpControllerState.Walking_Running:
            {
                isClimbing = false;
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
        }
    }

    //========================================================================
    public JumpControllerState SetJumpState()
    {
        if(PlayerTouchingGround())
        {
            return JumpControllerState.Walking_Running;
        }

        if (jumpControllerState == JumpControllerState.Climbing_DirectionChange)
        {
            return JumpControllerState.Climbing_DirectionChange;
        }

        if(jumpControllerState == JumpControllerState.Climbing_Step)
        {
            if(Vector2.Distance(rigidBody.position, climbStepTarget) < 0.01)
            {
                return JumpControllerState.Init_Climbing;
            }
            else 
            {
                return JumpControllerState.Climbing_Step;
            }
        }

        if(IsContactingWall())
        {
            if (isClimbing)
            {
                if (fallTimer > beginFallTime)
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

        if(!IsContactingWall() && isClimbing && !IsHeadAboveWall())
        {
            isClimbing = false;
            rigidBody.gravityScale = originalGravityScale;
            if (jumpTimer - timeLastJump < 0.15f)
            {
                float jumpForce = sprintController.isSprinting ? sprintJumpForce : normalJumpForce;
                rigidBody.AddForce(new Vector2(rigidBody.velocity.x, 1) * jumpForce, ForceMode2D.Impulse);
                jumpTimer = 0;
                Debug.Log("Jump Before Release");
                return JumpControllerState.Airborne;
            }
            return JumpControllerState.Climbing_DirectionChange;
        }

        return JumpControllerState.Airborne;
    } 

    //========================================================================
    public void Jump() 
    {        
        timeLastJump = jumpTimer;
        switch (jumpControllerState)
        {
            case JumpControllerState.Walking_Running:
            {
                float jumpForce = sprintController.isSprinting ? sprintJumpForce : normalJumpForce;
                rigidBody.AddForce(new Vector2(rigidBody.velocity.x, 1) * jumpForce, ForceMode2D.Impulse);
                break;
            }
            case JumpControllerState.Climbing_Falling:
            {
                rigidBody.gravityScale = 0;
                rigidBody.velocity = Vector2.zero;
                goto case JumpControllerState.Climbing;
            }
            case JumpControllerState.Climbing: 
            {
                climbStepTarget = rigidBody.position + new Vector2(0, 15f * Time.fixedDeltaTime);
                jumpControllerState = JumpControllerState.Climbing_Step;
                playerAnimator.SetJumpButtonPressed();
                break;
            }
            case JumpControllerState.Climbing_OverLedge:
            {
                jumpControllerState = JumpControllerState.Airborne;
                goto case JumpControllerState.Walking_Running;
            }
            case JumpControllerState.Climbing_DirectionChange:
            {
                jumpControllerState = JumpControllerState.Airborne;
                if (IsNearWall())
                {
                    Debug.Log("Jump After Release");
                    goto case JumpControllerState.Walking_Running;
                }
                break;
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
        float wallCheckDistance = 0.25f;
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, wallCheckDistance, ~LayerMask.GetMask("Player"));
        return hit.collider != null && hit.collider.CompareTag("Terrain");
    }    

    //========================================================================
    bool IsHeadAboveWall()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        Vector2 direction = wallDirection > 0 ? Vector2.right : Vector2.left;
        Vector2 rayStart = (Vector2)transform.position + new Vector2(0, playerCollider.bounds.extents.y); 
        float wallCheckDistance = 0.25f;
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, wallCheckDistance, ~LayerMask.GetMask("Player"));
        return hit.collider == null || !hit.collider.CompareTag("Terrain");
    }

    //========================================================================
    bool IsNearWall()
    {
        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection = wallDirection > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 10f, ~LayerMask.GetMask("Player"));
        return hit.collider != null && hit.distance < 1f && hit.collider.CompareTag("Terrain");
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
    Climbing_DirectionChange
}
