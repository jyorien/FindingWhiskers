using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    private BoxCollider2D boxCollider2D;
    private Animator animator;

    private bool isFacingRight = true;
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
    [SerializeField] private float maxJumpForce;
    [SerializeField] private float minJumpForce;
    [SerializeField] private LayerMask groundLayerMask;
    private bool isJump;

    [Header("Movement Speed")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxExtraSpeedOnIce;
    private float currentSpeedOnIce = 0;

    [Header("Size Scale")]
    [SerializeField] private float maxSizeScale = 1.3f;
    [SerializeField] private float minSizeScale = 0.6f;

    [Header("Gravity Scale")]
    [SerializeField] private float gravityScale;
    [SerializeField] private float fallGravityScale;

    [Header("Time to Minimum")]
    [SerializeField] private int timeTakenToReachMinimum = 5;

    [Header("Wall Slide")]
    [SerializeField] private float wallSlidingSpeed = 6f;
    [SerializeField] private Transform wallCheck;
    private bool isWallSliding;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f,12f);
    [SerializeField] private float wallJumpingTime = 0.2f;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingCounter;

    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponent<Animator>();

        decreaseValueAttributesOverTime = DecreaseAttributeValuesOverTime(timeTakenToReachMinimum);
        ResetToMaxAttributeValues();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!canMove) return;
        // get movement input to detect which direction player wants to move
        horizontalMovement = Input.GetAxisRaw("Horizontal");

        // set the jumping animation state based on whether player is touching ground
        animator.SetBool("Jumping", PlayerObstacleCollision.bottomColliderType != BottomColliderType.FLOOR);

        // get whether player wants to jump
        isJump = Input.GetButtonDown("Jump");
   
        // jumps if player presses jump button and is touching the ground
        Jump();
        // detect if player is wall sliding or wall jumping
        WallSlide();
        WallJump();

        // dont let player input control flipping when they wall jump
        if (!isWallJumping)
        {
            // flip player if not facing where they're supposed to
            if (isFacingRight && horizontalMovement < 0f || !isFacingRight && horizontalMovement > 0f)
            {
                FlipHorizontally();

            }
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;
        // dont let player input control flipping when they wall jump
        if (!isWallJumping)
        {
            /* player has different speed on ground and on ice.
             *  On ice, player does not stop moving immediately, but slowly comes to a stop
             */
            if (horizontalMovement != 0)
            {
                switch (PlayerObstacleCollision.groundType)
                {
                    case GroundType.DIRT:
                        // if player presses on a key to move left or right, add velocity
                        rigidBody2D.velocity = new Vector2(horizontalMovement * currentSpeed, rigidBody2D.velocity.y);
                        break;

                    case GroundType.ICE:
                        // constant acceleration until maximum velocity
                        if (currentSpeedOnIce < maxExtraSpeedOnIce)
                        {
                            currentSpeedOnIce += 0.2f;
                        }
                        rigidBody2D.velocity = new Vector2(horizontalMovement * currentSpeed, rigidBody2D.velocity.y);
                        // if player is on ice, make player go faster since ice has "less friction" in real life
                        rigidBody2D.velocity += new Vector2(horizontalMovement * currentSpeedOnIce, 0);
                        break;
                }
                animator.SetBool("Walking", true);
            }

            // when player is not trying to move, reset velocity to 0 if on dirt
            // if on ice, let the Ice PhysicsMaterial handle the deceleration
            else 
            {
                // reset the speed to accelerate when player moves again
                currentSpeedOnIce = 0;
                if (IsOnDirt())
                {
                    // player doesnt move on dirt when horizontalMovement == 0
                    rigidBody2D.velocity = new Vector2(horizontalMovement * currentSpeed, rigidBody2D.velocity.y);
                }
                animator.SetBool("Walking", false);
            }
        }
    }

    private void FlipHorizontally()
    {
        isFacingRight = !isFacingRight;
        transform.localRotation = isFacingRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }

    private bool IsOnDirt()
    {
        // use raycasting to do a ground check from the middle of the player to the bottom
        // note: dirt check checks for objects tagged with "Ground" (Dirt Floor, Dirt Wall)

        // add extra height to detect a bit below the player
        float heightOffset = 0.2f;

        // only allow 1 result to be returned by BoxCast
        RaycastHit2D[] raycastHits = new RaycastHit2D[1];

        /* create a ContactFilter2D so that the BoxCast only detects objects on the 'Ground' layer
         * and ignore trigger collisions so the player cannot jump when touching platforms that are in isTrigger
         */
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(groundLayerMask);
        contactFilter.useTriggers = false;

        Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, contactFilter, raycastHits, heightOffset);
        // store the only result returned into a variable for easy reference
        RaycastHit2D raycastHit2D = raycastHits[0];

        // to avoid NullPointerException, check whether collider is null first
        if (raycastHit2D.collider != null)
        {
            return raycastHit2D.collider.tag == "Ground" || raycastHit2D.collider.tag == "Platform";

        } else
        {
            // if null, immediately return false as player is definitely not on dirt
            return false;
        }
    }

    private bool IsTouchingWall()
    {
        bool isTouchingWall = false;
        /* determine if player is touching wall by checking colliders within a circlular area of Wall Check's transform.
         * detects walls based on their layer
         */
        Collider2D collider = Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayerMask);

        // We do not want player to wall slide or jump off ice or platforms, so make sure object in "Ground" layer is labelled "Ground" as all the dirt walls are tagged that
        if (collider && collider.tag == "Ground")
        {
          isTouchingWall = true;
        }
        return isTouchingWall;
    }
 
    private void Jump()
    {
        if (isJump && PlayerObstacleCollision.bottomColliderType == BottomColliderType.FLOOR)
        {
            rigidBody2D.AddForce(Vector2.up * currentJumpForce, ForceMode2D.Impulse);
        }
        // to achieve faster falling, change gravity to a higher value than when jumping up
        rigidBody2D.gravityScale = rigidBody2D.velocity.y > 0 ? rigidBody2D.gravityScale : fallGravityScale;
    }

    private void WallSlide()
    {
        /* allow player to slide down walls slowly if they are pressing movement keys while clinging onto a wall.
         * this helps with wall jumping
         */
        if (IsTouchingWall() && PlayerObstacleCollision.bottomColliderType != BottomColliderType.FLOOR && horizontalMovement != 0f)
        {
            isWallSliding = true;
            // negative speed to slide downwards
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Clamp(rigidBody2D.velocity.y, -wallSlidingSpeed, float.MaxValue));
        } else
        {
            isWallSliding = false;
        }
        animator.SetBool("Wall Sliding", isWallSliding);
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
            rigidBody2D.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            // set to 0 to prevent wall jumping more than once without touching another wall
            wallJumpingCounter = 0f;

            /* if not facing where its supposed to, flip the object
             * flipping will only be controlled here while wall jumping
             */
            if (wallJumpingDirection == 1 && transform.localRotation.y == -1 || wallJumpingDirection == -1 && transform.localRotation.y == 0)
            {
                FlipHorizontally();
            }
            
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
