using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    //========================================================================
    public float normalJumpForce;
    public float sprintJumpForce;
    public Rigidbody2D rigidBody;
    public PlayerInputManager inputManager;
    //========================================================================
    private SprintController sprintController;
    private GrappleController grappleController;

    //========================================================================
    void Start()
    {
        sprintController = inputManager.sprintController;
        grappleController = inputManager.grappleController;
    }

    //========================================================================
    public void Jump(bool forceJump = false) 
    {
        if(CanJump() || forceJump)
        {
            float jumpForce = sprintController.isSprinting ? sprintJumpForce : normalJumpForce;
            rigidBody.AddForce(new Vector2(rigidBody.velocity.x, 1) * jumpForce, ForceMode2D.Impulse);
        }
    }

    //======================================================================== 
    private bool CanJump()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        Vector2 rayStart = (Vector2)transform.position - new Vector2(0, playerCollider.bounds.extents.y); 
        Vector2 rayDirection = Vector2.down; 
        float rayLength = 0.1f;  

        RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, rayLength, ~LayerMask.GetMask("Player"));

        return hit.collider != null && hit.collider.CompareTag("Terrain") && !grappleController.isGrappling;
    }

    //======================================================================== 
    public bool IsJumping()
    {
        return !CanJump();
    }
}
