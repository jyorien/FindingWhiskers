using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float raycastDistance;
    [SerializeField] GameObject ghostProjectileToSpawn;
    [SerializeField] GameObject iceCubeProjectileToSpawn;
    [SerializeField] float projectileSpeed;
    [SerializeField] GameObject pathToWhiskers;

    private GameObject projectileSpawnPoint;
    private int layerMask;
    private IEnumerator fireAtPlayer;
    private bool isFiring;
    private Direction horizontalDirection = Direction.Left;
    private Rigidbody2D rb;
    // keep track of how many times the player hit Ghost
    private int hitCount = 0;
    // Ghost will be given 3 lives
    private const int livesCount = 3;
    // Determines if Ghost can turn his transform upon colliding with a wall
    private bool canTurn = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();

        // use a LayerMask for Ghost to only detect Player in his line of sight
        layerMask = 1 << LayerMask.NameToLayer("Player");
        // store the projectile spawn point's gameObject so we can access its position later
        projectileSpawnPoint = gameObject.transform.GetChild(0).gameObject;
    }

    private void FixedUpdate()
    {
       if (isPlayerInSight())
        {
            // force Ghost to stop moving when firing
            rb.velocity = new Vector2(0, rb.velocity.y);
            // if already firing, don't start the coroutine
            if (isFiring) return;

            // start firing at player when Ghost sees player
            fireAtPlayer = FireAtPlayer();
            StartCoroutine(fireAtPlayer);

            // flag that Ghost is already firing
            isFiring = true;
        }
        else
        {
            // if Ghost was firing, stop firing since player is out of sight
            if (isFiring)
            {
                StopCoroutine(fireAtPlayer);
                isFiring = false;
            }

            int horizontalMovement;

            // set value of the direction based on enum
            switch (horizontalDirection)
            {
                // since Ghost's sprite is facing left, by default it will have negative velocity
                case Direction.Left:
                default:
                    horizontalMovement = -1;
                    break;
                // positive velocity makes Trunk move to the right
                case Direction.Right:
                    horizontalMovement = 1;
                    break;
            }
            rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
        }
    }

    IEnumerator TurningCooldown()
    {
        /* when player turns, it might trigger another OnCollisionEnter2D event which might cause Ghost to turn endlessly
         * set a cooldown period so Ghost can move away from the wall before turning again
         */
        canTurn = false;
        yield return new WaitForSeconds(0.5f);
        canTurn = true;
    }

    bool isPlayerInSight()
    {
        /* draw ray from Ghost towards the right and limit the ray's distance. 
         * The ray will only collide with objects in the Player layer since 
         * we only want the Ghost to be hostile when the player is in the Ghost's line of sight 
         */
        RaycastHit2D hit2d = Physics2D.Raycast(transform.position, -transform.right, raycastDistance, layerMask);
        if (hit2d.collider !=  null)
        {
            Debug.DrawLine(transform.position, hit2d.point, Color.red);
            return true;
        }
        return false;
    }

    void FireProjectile()
    {
        // get location to spawn projectile
        Vector3 projectileSpawnPointLocation = projectileSpawnPoint.transform.position;

        // randomly create either a normal or ice cube projectile to spawn
        GameObject newProjectile;
        switch (Random.Range(0,2))
        {
            case 0:
            default:
                newProjectile = Instantiate(ghostProjectileToSpawn, projectileSpawnPointLocation, Quaternion.identity);
                break;
            case 1:
                newProjectile = Instantiate(iceCubeProjectileToSpawn, projectileSpawnPointLocation, Quaternion.identity);
                break;
        }

        // add force to push projectile forward
        Rigidbody2D projectileRb = newProjectile.GetComponent<Rigidbody2D>();

        // determine direction the force should be applied to projectile based on where Ghost is facing
        Vector2 forceDirection;
        switch (horizontalDirection)
        {
            case Direction.Left:
            default:
                forceDirection = Vector2.left;
                break;
            case Direction.Right:
                forceDirection = Vector2.right;
                break;
        }
        projectileRb.AddForce(forceDirection * projectileSpeed);
        
        // destroy projectile after a few seconds to avoid keeping too many unused GameObjects
        Destroy(newProjectile, 3);
    }

    IEnumerator FireAtPlayer()
    {
        // keep firing infinitely with a set delay
        while (true)
        {
            /* put the delay first so projectiles don't spam
             * when player keeps re-entering the line of sight
             */
            yield return new WaitForSeconds(1.5f);
            FireProjectile();
        }
    }

    /* As some colliders are children of the GameObject,
     * pass this method to each child to handle their collisions here.
     * 
     * ColliderSide will be used to identify which child the collision was from
     */
    public void OnChildCollisionDetected(ColliderSide childColliderSide, Collision2D collision)
    {
        string colliderTag = collision.collider.tag;
            switch (childColliderSide)
            {
                /* if player touches enemy on top collider, count as a hit
                 * if Ghost gets hit 3 times, the platforms leading to whiskers show up
                 * for the player to complete the level
                 */
                case ColliderSide.Top:
                    if (colliderTag == "Player")
                    {
                        hitCount += 1;

                        // if Ghost has been defeated
                        if (hitCount > livesCount - 1)
                        {
                            GameManager.Instance.isLevelCompleteRequirementMet = true;
                            Destroy(gameObject);
                            pathToWhiskers.SetActive(true);
                        }
                    }
                    break;

                // if player touches enemy by the sides, player loses a life
                case ColliderSide.Side:
                    if (colliderTag == "Player")
                    {
                        Destroy(collision.gameObject);
                        GameManager.Instance.OnGameLose();
                    }
                    if (colliderTag == "Ice")
                    {
                    if (canTurn)
                    {
                        // flip the horizontal direction if Ghost bumps into wall
                        horizontalDirection = horizontalDirection == Direction.Left ? Direction.Right : Direction.Left;
                        switch (horizontalDirection)
                        {
                            // rotate transform when changing directions
                            case Direction.Left:
                            default:
                                transform.localRotation = Quaternion.Euler(0, 0, 0);
                                break;
                            case Direction.Right:
                                transform.localRotation = Quaternion.Euler(0, 180, 0);
                                break;
                        }
                        StartCoroutine(TurningCooldown());
                    }
                    
                    }
                    break;
            }
       


    }
}
