using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkEnemy : MonoBehaviour
{
    [SerializeField] float speed;
    private Rigidbody2D rb;

    // use enum to make direction more readable
    private Direction horizontalDirection = Direction.Left;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int horizontalMovement;

        // set value of the direction based on enum
        switch (horizontalDirection)
        {
            // since Trunk's sprite is facing left, by default it will have negative velocity
            case Direction.Left:
            default:
                horizontalMovement = -1;
                break;
            // positive velocity makes Trunk move to the right
            case Direction.Right:
                horizontalMovement = 1;
                break;
        }
        rb.velocity = new Vector2(horizontalMovement * speed, rb.velocity.y);
    }

    /* As some colliders are children of the GameObject,
     * pass this method to each child to handle their collisions here.
     * 
     * ColliderSide will be used to identify which child the collision was from
     */
    public void OnChildCollisionDetected(ColliderSide childColliderSide, Collision2D collision)
    {
        string colliderTag = collision.collider.tag;
        if (colliderTag == "Player")
        {
            switch (childColliderSide)
            {
                // if player touches enemy on top collider, enemy gets destroyed and the player bounces off
                case ColliderSide.Top:
                    Destroy(gameObject);
                    break;

                // if player touches enemy by the sides, player loses a life
                case ColliderSide.Side:
                    Destroy(collision.gameObject);
                    GameManager.Instance.OnGameLose();
                    break;
            }
        }
        if (colliderTag == "Ground" || colliderTag == "InstantDeath" || colliderTag == "Ice")
        {
            // flip the horizontal direction if Trunk bumps into wall or spikes
            horizontalDirection = horizontalDirection == Direction.Left ? Direction.Right : Direction.Left;
        }
       

    }
}
