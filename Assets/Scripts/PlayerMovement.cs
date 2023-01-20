using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isFacingRight = false;
    // check whether player should be able to move
    /* player should not be able to move after they touch the End Pole 
    so that they do not move when the Level Complete Scene is loaded */
    public bool canMove = true;

    float horizontalMovement;
    bool isJump;
    bool isGrounded = false;

    /* store max attributes. player will always start off with max values
    and gradually decrease to min */
    [SerializeField] float maxJumpForce;
    [SerializeField] float maxSpeed;
    // max size will always stay the same
    const float maxSizeScale = 1.3f;

    // store min attributes. This will put a cap on how much the values can decrease
    [SerializeField] float minJumpForce;
    [SerializeField] float minSpeed;
    // min size will always stay the same
    const float minSizeScale = 0.6f;

    [SerializeField] float gravityScale;
    [SerializeField] float fallGravityScale;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D playerCollider;

    // actual values being used for each attribute
    private float currentJumpForce;
    private float currentSpeed;
    private float currentSizeScale;

    private void Start()
    {
        ResetToMaxAttributeValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;
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
            rb.AddForce(Vector2.up * currentJumpForce, ForceMode2D.Impulse);
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
        if (!canMove) return;
        // move player horizontally based on speed in inspector
        rb.velocity = new Vector2(horizontalMovement * currentSpeed, rb.velocity.y);
        
    }

    public void ResetToMaxAttributeValues()
    {
        /* when starting the game or touching a campfire,
        reset jump force, speed and size */
        currentJumpForce = maxJumpForce;
        currentSpeed = maxSpeed;
        currentSizeScale = maxSizeScale;
    }

    /* As some colliders are children of the GameObject, 
     pass this method to each child to handle their collisions here.
    ColliderSide will be used to identify which child the collision was from
     */
    public void OnChildCollisionDetected(ColliderSide childColliderSide, Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            case "Ground":
                // reset jump when player lands on ground
                isGrounded = true;
                break;
            case "Ice":
                // only reset jump if player lands on ice ground (touching ice wall does not reset)
                if (childColliderSide == ColliderSide.Bottom)
                    isGrounded = true;
                break;
        }

    }
}

public enum ColliderSide
{
    Bottom,
    Side
}
