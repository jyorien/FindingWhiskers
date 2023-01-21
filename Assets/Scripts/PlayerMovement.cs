using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isFacingRight = false;

    /* check whether player should be able to move.
     * 
     * player should not be able to move after they touch the End Pole 
     * so that they do not move when the Level Complete Scene is loaded
     */
    public bool canMove = true;

    float horizontalMovement;
    bool isJump;
    bool isGrounded = false;

    /* store max and min attributes. player will always start off with max values
     * and gradually decrease to min. 
     */
    [SerializeField] float maxJumpForce;
    [SerializeField] float minJumpForce;

    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;

    // min and max scale will always stay the same
    const float maxSizeScale = 1.3f;
    const float minSizeScale = 0.6f;

    [SerializeField] float gravityScale;
    [SerializeField] float fallGravityScale;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D playerCollider;
    [SerializeField] int timeTakenToReachMinimum = 5;

    // actual values being used for each attribute
    private float currentJumpForce;
    private float currentSpeed;

    // store coroutine in variable so we can control start/stopping it
    private IEnumerator decreaseValueAttributesOverTime;

    private void Start()
    {
        decreaseValueAttributesOverTime = DecreaseAttributeValuesOverTime(timeTakenToReachMinimum);
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
         * when jumping up 
         */
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

    IEnumerator DecreaseAttributeValuesOverTime(int durationInSeconds)
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

    /* As some colliders are children of the GameObject,
     * pass this method to each child to handle their collisions here.
     * 
     * ColliderSide will be used to identify which child the collision was from
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
