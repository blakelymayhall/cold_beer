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

    public MovementController movementController;
    public JumpController jumpController;
    public SprintController sprintController;
    public GrappleController grappleController;
    public SpriteRenderer spriteRenderer;
    //========================================================================
    private List<Sprite> activeSprites;
    private float timer = 0f;
    private float walkTime = 0.2f;
    private float runTime = 0.15f;
    private int spriteIndex = 1;
    private float lastMoveInput = 0;
    private bool jumpButtonPressed = false;
    private Sprite lastGrappleSprite;
    //========================================================================

    void Start()
    {        
    }

    //========================================================================
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        // Handle idle state
        if (movementController.movementInput == 0)
        {
            ResetToIdle();
            return;
        }

        // Handle direction change
        if (spriteRenderer.sprite == stillSprite || 
            Mathf.Sign(movementController.movementInput) != Mathf.Sign(lastMoveInput))
        {
            lastMoveInput = movementController.movementInput;
            bool walkingRight = movementController.movementInput > 0;
            activeSprites = walkingRight ? movingRightSprites : movingLeftSprites;
            spriteRenderer.sprite = activeSprites[spriteIndex];
        }

        // Jumping
        if(jumpController.IsJumping() && !grappleController.isGrappling)
        {
            spriteRenderer.sprite = activeSprites[0];
            return;
        }

        // Grappling 
        if(jumpController.IsJumping() && grappleController.isGrappling)
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

            return;
        }

        // Walking / Sprinting 
        float animationTime = sprintController.isSprinting ? runTime : walkTime;
        if (timer > animationTime)
        {
            UpdateSpriteAnimation();
        }
    }

    //========================================================================
    public void SetJumpButtonPressed()
    {
        jumpButtonPressed = true;
    }

    //========================================================================
    void ResetToIdle()
    {
        timer = 0f;
        spriteRenderer.sprite = stillSprite;
        spriteIndex = 1;
    }

    //========================================================================
    void UpdateSpriteAnimation()
    {
        timer = 0f;
        spriteIndex = (spriteIndex + 1) % activeSprites.Count;
        spriteRenderer.sprite = activeSprites[spriteIndex];
    } 
}
