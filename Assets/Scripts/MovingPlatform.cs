using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // can select Horizontal / Vertical movement in inspector
    [SerializeField] Axis axis;

    // adjust how fast the platform moves in inspector
    [SerializeField] float speed;

    /* store whether the platform should move forward or backward
    to keep within the boundaries of the start and end point */
    bool isReverse = false;

    private void Update()
    {
        //  determines if platform moves forward or backward
        float velocity = isReverse ? speed * -1 : speed;

        // platform is only allowed to move around the x or y axis
        switch (axis)
        {
            case Axis.Horizontal:
                transform.Translate(new Vector3(velocity * Time.deltaTime, 0, 0));
                break;
            case Axis.Vertical:
                transform.Translate(new Vector3(0, velocity * Time.deltaTime, 0));
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float yVelocityOfIncomingCollider = collision.rigidbody.velocity.y;
        float yPositionOfIncomingCollider = collision.transform.position.y;

        float yPositionOffPlatform = gameObject.transform.position.y;

        // only set object's parent to platform IF object is not jumping (y velocity = 0)
        // AND IF the object is on top of the platform (y position of object above y position of platform)
        if (yVelocityOfIncomingCollider == 0 && yPositionOfIncomingCollider > yPositionOffPlatform)
        {
            switch (collision.collider.tag)
            {
                // make player move with platform
                case "Player":
                    collision.collider.transform.parent.SetParent(transform);
                    break;
            }
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            /* detach player from platform
            and let the player go back to moving independently */
            case "Player":
                collision.collider.transform.parent.SetParent(null);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // store into variable for easy reference
        string collisionName = collision.name;

        // change direction of platform movement when it hits the start or end
        if (collisionName == "Start Point" || collisionName == "End Point")
        {
            isReverse = !isReverse;
        }
    }

}

// create enum to get dropdown list in inspector
enum Axis
{
    Horizontal,
    Vertical
}
