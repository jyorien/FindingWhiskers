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
    [SerializeField] LayerMask layerMask;

    private GameObject projectileSpawnPoint;
    // use a LayerMask for Ghost to only detect Player in his line of sight
    private IEnumerator fireAtPlayer;
    private bool isFiring;
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
            // move forward
            rb.velocity = -transform.right * moveSpeed;
        }
    }

    IEnumerator TurningCooldown()
    {
        /* when Ghost turns, it might trigger another OnCollisionEnter2D event which might cause Ghost to turn endlessly
         * set a cooldown period so Ghost can move away from the wall before turning again
         */
        canTurn = false;
        yield return new WaitForSeconds(0.5f);
        canTurn = true;
    }

    bool isPlayerInSight()
    {
        /* draw ray from in front of Ghost and limit the ray's distance. 
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

        // shoot projectile with force in the direction Ghost is facing
        projectileRb.AddForce(-transform.right * projectileSpeed);
        
        // destroy projectile after a few seconds to avoid keeping too many unused GameObjects
        Destroy(newProjectile, 3);
    }

    IEnumerator FireAtPlayer()
    {
        /* add a short delay before the loop so when player jumps to re-enter the line of sight,
         * Ghost doesn't immediately start firing
         */
        yield return new WaitForSeconds(0.5f);
        // keep firing infinitely with a set delay
        while (true)
        {
            // randomly spawn normal or ice cube projectile every 1.5s until coroutine gets stopped
            FireProjectile();
            yield return new WaitForSeconds(1.5f);
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

                case ColliderSide.Side:
                    switch (colliderTag)
                    {
                        // if player touches enemy by the sides, player loses a life
                        case "Player":
                        Destroy(collision.gameObject);
                        GameManager.Instance.OnGameLose();
                        break;

                        // flip the horizontal direction if Ghost bumps into ice wall
                        case "Ice":
                        if (canTurn)
                        {
                           transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
                           StartCoroutine(TurningCooldown());
                        }
                        break;
                    }
                    break;
            }
    }
}
