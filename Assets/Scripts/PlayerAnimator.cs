using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerAnimator : MonoBehaviour
{
    //========================================================================
    public List<Sprite> movingLeftSprites = new();
    public List<Sprite> movingRightSprites = new();
    public Sprite stillSprite;
    public SpriteRenderer spriteRenderer;
    public PlayerInputManager inputManager;
    public PlayerAnimationType playerAnimationState;
    //========================================================================
    private MovementController movementController;
    private JumpController jumpController;
    private SprintController sprintController;
    private GrappleController grappleController;
    private List<Sprite> activeSprites;
    private float timer = 0f;
    private int spriteIndex = 1;
    private float lastMoveInput = 0;
    private bool jumpButtonPressed = false;
    private Sprite lastGrappleSprite;

    private const float walkTime = 0.2f;
    private const float runTime = 0.15f;
    //========================================================================

    void Start()
    {        
        sprintController = inputManager.sprintController;
        grappleController = inputManager.grappleController;
        jumpController = inputManager.jumpController;
        movementController = inputManager.movementController;
    }

    //========================================================================
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        playerAnimationState = SetAnimationState();

        switch (playerAnimationState)
        {
            case PlayerAnimationType.Still:
                ResetToIdle();
                break;
            case PlayerAnimationType.Walking_Running:
                ChangeMovingDirection();
                Walking_Running_Animation();
                break;
            case PlayerAnimationType.Jumping:
                ChangeMovingDirection();
                spriteRenderer.sprite = activeSprites[0];
                break;
            case PlayerAnimationType.Climbing:
                ChangeMovingDirection();
                ClimbStepAnimation();
                break;
            case PlayerAnimationType.Climbing_Falling:
                break;
            default:
                ResetToIdle();
                break;
        }
    }

    //========================================================================
    public PlayerAnimationType SetAnimationState()
    {
        if (movementController.movementInput == 0)
        {
            return PlayerAnimationType.Still;
        }

        if (jumpController.IsJumping() && !grappleController.isGrappling)
        {
            return PlayerAnimationType.Jumping;
        }

        if (jumpController.IsJumping() && grappleController.isGrappling)
        {
            return PlayerAnimationType.Climbing;
        }

        return PlayerAnimationType.Walking_Running;
    }

    //========================================================================
    public void ChangeMovingDirection()
    {
        if (spriteRenderer.sprite == stillSprite || 
            Mathf.Sign(movementController.movementInput) != Mathf.Sign(lastMoveInput))
        {
            lastMoveInput = movementController.movementInput;
            bool walkingRight = movementController.movementInput > 0;
            activeSprites = walkingRight ? movingRightSprites : movingLeftSprites;
            spriteRenderer.sprite = activeSprites[spriteIndex];
        }
    }

    //========================================================================
    public void SetJumpButtonPressed()
    {
        jumpButtonPressed = true;
    }

    //========================================================================
    void ClimbStepAnimation()
    {
        if (jumpButtonPressed)
        {
            timer = 0f;
            if(lastGrappleSprite == activeSprites[1])
            {
                spriteRenderer.sprite = activeSprites[3];
            }
            else 
            {
                spriteRenderer.sprite = activeSprites[1];
            }
            lastGrappleSprite = spriteRenderer.sprite;
            jumpButtonPressed = false;
        }

        if (timer > walkTime)
        {
            spriteRenderer.sprite = activeSprites[0];
        }
    }

    //========================================================================
    void ResetToIdle()
    {
        timer = 0f;
        spriteRenderer.sprite = stillSprite;
        spriteIndex = 1;
    }

    //========================================================================
    void Walking_Running_Animation()
    {
        float animationTime = sprintController.isSprinting ? runTime : walkTime;
        if (timer > animationTime)
        {
            timer = 0f;
            spriteIndex = (spriteIndex + 1) % activeSprites.Count;
            spriteRenderer.sprite = activeSprites[spriteIndex];
        }
    } 
}
