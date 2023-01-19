using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isFacingRight = false;

    float horizontalMovement;
    bool isJump;
    bool isGrounded = false;
    [SerializeField] float jumpForce;
    [SerializeField] float gravityScale;
    [SerializeField] float fallGravityScale;
    [SerializeField] float speed;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D playerCollider;


    // Update is called once per frame
    void Update()
    {
        // get movement input to detect which direction player wants to move
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        if (horizontalMovement == 1)
        {
            isFacingRight = true;
        } else if (horizontalMovement == -1)
        {
            isFacingRight = false;
        }

        isJump = Input.GetButtonDown("Jump");
            
        if (isJump && isGrounded)
        {
            isGrounded = false;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        /* to achieve faster falling, change gravity to a higher value than
        when jumping up */
        if (rb.velocity.y > 0)
        {
            rb.gravityScale = gravityScale;
        } else
        {
            rb.gravityScale = fallGravityScale;
        }
       

    }

    private void FixedUpdate()
    {
        // move player horizontally based on speed in inspector
        rb.velocity = new Vector2(horizontalMovement * speed, rb.velocity.y);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            // reset jump count so player can jump again
            case "Ground":
                isGrounded = true;
                break;
            case "Ice":
                if (rb.velocity.y == 0)
                {
                    isGrounded = true;

                }
                break;
        }
        
    }
}
