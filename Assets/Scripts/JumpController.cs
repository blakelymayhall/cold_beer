using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    //========================================================================
    public float jumpForce = 500f;
    public Rigidbody2D rb;
    //========================================================================
    private bool canJump = true;

    //========================================================================
    void Start()
    {
        
    }

    //========================================================================
    public void Jump() 
    {
        if(CanJump())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canJump = false;
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

        return hit.collider != null && hit.collider.CompareTag("Terrain");
    }
}
