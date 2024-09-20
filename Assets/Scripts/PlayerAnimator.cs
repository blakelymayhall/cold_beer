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
    public Rigidbody2D rigidBody;
    public SpriteRenderer spriteRenderer;
    //========================================================================
    private List<Sprite> activeSprites;
    private float timer = 0f;
    private float walkTime = 0.2f;
    private float runTime = 0.15f;
    private int spriteIndex = 1;
    private float lastMoveInput = 0;
    //========================================================================

    void Start()
    {        
    }

    //========================================================================
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (movementController.movementInput == 0)
        {
            timer = 0f;
            spriteRenderer.sprite = stillSprite;
            spriteIndex = 1;
        }

        if(movementController.movementInput != 0 && (spriteRenderer.sprite == stillSprite || 
            Mathf.Sign(movementController.movementInput) != Mathf.Sign(lastMoveInput)) ) 
        {
            lastMoveInput = movementController.movementInput;
            bool walkingRight = movementController.movementInput > 0;
            activeSprites =  walkingRight ? movingRightSprites : movingLeftSprites;
            spriteRenderer.sprite = activeSprites[spriteIndex];
        }
        else if (timer > (sprintController.isSprinting ? runTime : walkTime))
        {
            timer = 0f;
            spriteIndex = (spriteIndex + 1) % activeSprites.Count;
            spriteRenderer.sprite =  jumpController.IsJumping() ? activeSprites[0] : activeSprites[spriteIndex];
        }
    }
}
