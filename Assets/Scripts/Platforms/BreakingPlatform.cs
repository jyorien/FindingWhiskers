using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingPlatform : MonoBehaviour
{
    BoxCollider2D platformCollider;
    SpriteRenderer spriteRenderer;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        platformCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // make collider isTrigger to let player pass through
        platformCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        float yVelocityOfIncomingCollider = collision.attachedRigidbody.velocity.y;
        float yPositionOfIncomingCollider = collision.attachedRigidbody.position.y;

        float yPositionOffPlatform = gameObject.transform.position.y;

        /* only enable collider for player to stand on platform if player is walking horizontally (zero y velocity)
         * or falling downwards (negative y velocity)
         */
        if (yVelocityOfIncomingCollider <= 0 && yPositionOfIncomingCollider > yPositionOffPlatform)
        {
            switch (collision.tag)
            {
                case "Player":
                    platformCollider.isTrigger = false;
                // start timer to break platform when player is on it
                    StartCoroutine(BreakPlatform());
                    break;
            }
        }
    }

    IEnumerator BreakPlatform()
    {
        animator.SetBool("Breaking", true);
        // break platform after a second
        yield return new WaitForSeconds(1);
        SetPlatformActive(false);
        // respawn after two seconds
        yield return new WaitForSeconds(2);
        animator.SetBool("Breaking", false);
        SetPlatformActive(true);
    }

    void SetPlatformActive(bool isActive)
    {
        // set state of collider and sprite to give platform's "breaking" effect
        platformCollider.enabled = isActive;
        spriteRenderer.enabled = isActive;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // make platform unable to pass through again when player leaves platform
        platformCollider.isTrigger = true;

    }
}
