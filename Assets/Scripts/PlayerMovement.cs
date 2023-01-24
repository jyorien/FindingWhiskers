using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isFacingRight = true;
    private float horizontalMovement;

    // store coroutine in variable so we can control start/stopping it
    private IEnumerator decreaseValueAttributesOverTime;

    /* check whether player should be able to move.
     * 
     * player should not be able to move after they touch the End Pole 
     * so that they do not move when the Level Complete Scene is loaded
     */
    public bool canMove = true;
    // actual values being used for each attribute
    private float currentJumpForce;
    private float currentSpeed;

    /* store max and min attributes. player will always start off with max values
     * and gradually decrease to min. 
     */
    [Header("Jump")]
    [SerializeField] float maxJumpForce;
    [SerializeField] float minJumpForce;
    [SerializeField] LayerMask groundLayerMask;
    private bool isJump;

    [Header("Movement Speed")]
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;

    [Header("Size Scale")]
    [SerializeField] float maxSizeScale = 1.3f;
    [SerializeField] float minSizeScale = 0.6f;

    [Header("Gravity Scale")]
    [SerializeField] float gravityScale;
    [SerializeField] float fallGravityScale;

    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D playerCollider;

    [Header("Time to Minimum")]
    [SerializeField] int timeTakenToReachMinimum = 5;

    [Header("Wall Slide")]
    [SerializeField] float wallSlidingSpeed = 6f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayerMask;
    private bool isWallSliding;

    [Header("Wall Jump")]
    [SerializeField] Vector2 wallJumpingPower = new Vector2(8f,12f);
    [SerializeField] float wallJumpingTime = 0.2f;
    [SerializeField] float wallJumpingDuration = 0.4f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingCounter;

    private void Start()
    {
        decreaseValueAttributesOverTime = DecreaseAttributeValuesOverTime(timeTakenToReachMinimum);
        ResetToMaxAttributeValues();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!canMove) return;
        // get movement input to detect which direction player wants to move
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        
        // get whether player wants to jump
        isJump = Input.GetButtonDown("Jump");
        // jumps if player presses jump button and is touching the ground
        Jump();
        // detect if player is wall sliding or wall jumping
        WallSlide();
        WallJump();

        // dont let player control flipping when they wall jump
        if (!isWallJumping)
        {
            Debug.Log($"horizontalMovement: {horizontalMovement}, isFacingRight: {isFacingRight}");
            // if facing right but going left or facing left but going right, flip player transform
            if (isFacingRight && horizontalMovement < 0f || !isFacingRight && horizontalMovement > 0f)
            {
                FlipHorizontally();

            }
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;
        // move player horizontally based on speed in inspector
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontalMovement * currentSpeed, rb.velocity.y);
        }
        
    }

    private void FlipHorizontally()
    {
        isFacingRight = !isFacingRight; 
        if (isFacingRight)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        } else
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private bool isGrounded()
    {
        // use raycasting to do a ground check from the middle of the player to the bottom

        // add extra height to detect a bit below the player
        float heightOffset = 0.2f;

        RaycastHit2D raycastHit2D = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, heightOffset, groundLayerMask);
        Color rayColor;
        if (raycastHit2D.collider != null)
        {
            rayColor = Color.red;
        } else
        {
            rayColor = Color.white;
        }
        // draw where the BoxCast hits
        Debug.DrawRay(playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + heightOffset), rayColor);
        Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + heightOffset), rayColor);
        Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, playerCollider.bounds.extents.y + heightOffset), Vector2.right * playerCollider.bounds.extents.x * 2, rayColor);
        return raycastHit2D.collider != null;
    }

    private bool isTouchingWall()
    {
        /* determine if player is touching wall by checking colliders within a circlular area of Wall Check's transform.
         * detects walls based on their layer mask
         */
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayerMask);
    }

    private void Jump()
    {
        if (isJump && isGrounded())
        {
            rb.AddForce(Vector2.up * currentJumpForce, ForceMode2D.Impulse);
        }
        /* to achieve faster falling, change gravity to a higher value than
         * when jumping up 
         */
        rb.gravityScale = rb.velocity.y > 0 ? rb.gravityScale : fallGravityScale;
    }

    private void WallSlide()
    {
        /* allow player to slide down walls slowly if they are pressing movement keys while clinging onto a wall.
         * this helps with wall jumping
         */
        if (isTouchingWall() && !isGrounded() && horizontalMovement != 0f)
        {
            isWallSliding = true;
            // negative speed to slide downwards
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        } else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            // jump in opposite direction
            wallJumpingDirection = isFacingRight ? -1 : 1;
            // reset counter if player is sliding
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            // if player is trying to wall jump, there will be a brief moment where turn away from the wall
            // count down until player is considered not trying to wall jump 
            wallJumpingCounter -= Time.deltaTime;
        }

        // if player clicks jump during the brief period of turning away from the wall, jump in the opposite direction
        if (isJump && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            // set to 0 to prevent wall jumping more than once
            wallJumpingCounter = 0f;
            
             FlipHorizontally();
            
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    public void ResetToMaxAttributeValues()
    {
        // reset the coroutine
        StopCoroutine(decreaseValueAttributesOverTime);
        decreaseValueAttributesOverTime = DecreaseAttributeValuesOverTime(timeTakenToReachMinimum);

        /* when starting the game or touching a campfire,
         * reset jump force, speed and size
         */
        currentJumpForce = maxJumpForce;
        currentSpeed = maxSpeed;
        transform.localScale = new Vector3(maxSizeScale, maxSizeScale, 1);

        // restart the coroutine
        StartCoroutine(decreaseValueAttributesOverTime);
    }

    private IEnumerator DecreaseAttributeValuesOverTime(int durationInSeconds)
    {
        float timePassed = 0f;
        Vector3 maxScaleVector3 = new Vector3(maxSizeScale, maxSizeScale, 1);
        Vector3 minScaleVector3 = new Vector3(minSizeScale, minSizeScale, 1);

        while (timePassed <= durationInSeconds)
        {
            timePassed += Time.deltaTime;

            /* scale the attribute logarithmically based on how much the time has passed compared to how long its supposed
             * to take to reach the minimum value of the attribute.
             *
             * scaling it logarithmically makes it decrease faster at the start 
             * and decrease slower towards the end
             */
            float t = Mathf.Log10(1 + timePassed) / Mathf.Log10(1 + durationInSeconds);

            transform.localScale = Vector3.Lerp(maxScaleVector3, minScaleVector3, t);
            currentJumpForce = Mathf.Lerp(maxJumpForce, minJumpForce, t);
            currentSpeed = Mathf.Lerp(maxSpeed, minSpeed, t);
            yield return null;
        }
    }
}
