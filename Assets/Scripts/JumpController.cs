using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    //========================================================================
    public float jumpForce = 500f;
    public Rigidbody2D rb;
    //========================================================================

    //========================================================================
    void Start()
    {
        
    }

    //========================================================================
    public void Jump() 
    {
       rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

}
