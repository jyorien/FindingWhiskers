using UnityEngine;

public class MovingPlatform : Platform
{
    // can select Horizontal / Vertical movement in inspector to make the platform move in that axis
    [SerializeField] private Axis axis;
    // adjust in inspector how fast the platform moves 
    [SerializeField] private float speed;

    // store whether the platform should move forward or backward
    // to keep within the boundaries of the start and end point
    private bool isReverse = false;

    private void Update()
    {
        // determines the speed and direction of the moving platform
        float velocity = isReverse ? speed * -1 : speed;

        // platform is only allowed to move around either the x or y axis
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

    protected override void OnPlayerStandingOnPlatform(Collider2D collision)
    {
        // allow player's transform to move with the platform 
        collision.transform.SetParent(transform);
    }

    protected override void OnPlayerLeavesPlatform(Collision2D collision)
    {
        // un-parent player from platform and let the player go back to moving independently
        collision.collider.transform.SetParent(null);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

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
