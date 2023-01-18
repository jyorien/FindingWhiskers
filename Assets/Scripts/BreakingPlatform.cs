using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingPlatform : MonoBehaviour
{
    BoxCollider2D platformCollider;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        platformCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        float yVelocityOfIncomingCollider = collision.rigidbody.velocity.y;
        float yPositionOfIncomingCollider = collision.transform.position.y;

        float yPositionOffPlatform = gameObject.transform.position.y;

        // only start to break IF object is not jumping (y velocity = 0)
        // AND IF the object is on top of the platform (y position of object above y position of platform)
        if (yVelocityOfIncomingCollider == 0 && yPositionOfIncomingCollider > yPositionOffPlatform)
        {
            switch (collision.collider.tag)
            {
                // start timer to break platform when player is on it
                case "Player":
                    StartCoroutine(BreakPlatform());
                    break;
            }
        }
        
    }

    IEnumerator BreakPlatform()
    {
        // break platform after a second
        yield return new WaitForSeconds(1);
        SetPlatformActive(false);
        // respawn after two seconds
        yield return new WaitForSeconds(2);
        SetPlatformActive(true);
    }

    void SetPlatformActive(bool isActive)
    {
        // set state of collider and sprite to give platform's "breaking" effect
        platformCollider.enabled = isActive;
        spriteRenderer.enabled = isActive;
    }
}
