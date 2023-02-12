using UnityEngine;

/// <summary>
/// This is the base class for creating a platform and it handles the logic of the player being able to walk through
/// the platform until they jump above the platform to stand on it.
/// It exposes callback methods for the platforms to handle the events when the player enters or leaves the platform.
/// It also exposes the platform's collider to the subclasses and hence requires a BoxCollider2D.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Platform : MonoBehaviour
{
    // expose the BoxCollider2D to subclasses
    protected BoxCollider2D platformCollider;

    protected virtual void Start()
    {
        platformCollider = gameObject.GetComponent<BoxCollider2D>();

        // make collider isTrigger to let player pass through
        platformCollider.isTrigger = true;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float yVelocityOfIncomingCollider = collision.attachedRigidbody.velocity.y;
            float yPositionOfIncomingCollider = collision.attachedRigidbody.position.y;

            float yPositionOffPlatform = gameObject.transform.position.y;

            // only enable collider for player to stand on platform if player is falling downwards (negative y velocity)
            // and the player is above the platform (player's y position greater than platform's y position)
            if (yVelocityOfIncomingCollider < 0 && yPositionOfIncomingCollider > yPositionOffPlatform)
            {
                platformCollider.isTrigger = false;
                OnPlayerStandingOnPlatform(collision);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            platformCollider.isTrigger = true;
            OnPlayerLeavesPlatform(collision);
        }
    
    }

    /// <summary>
    /// This method provides a callback to the subclasses to handle the event when the player stands on top of the platform.
    /// </summary>
    /// <param name="collision">Exposes the player's collider for the subclass to handle.</param>
    ///
    protected abstract void OnPlayerStandingOnPlatform(Collider2D collision);

    /// <summary>
    /// This method provides a callback to the subclasses to handle the event when the player leaves the platform.
    /// </summary>
    /// <param name="collision">Exposes information about the collision for subclass to handle.</param>
    protected abstract void OnPlayerLeavesPlatform(Collision2D collision);
}
