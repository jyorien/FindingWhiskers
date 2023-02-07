using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] float moveSpeed;
    [SerializeField] float raycastDistance;
    [SerializeField] GameObject ghostProjectileToSpawn;
    [SerializeField] GameObject iceCubeProjectileToSpawn;
    [SerializeField] float projectileSpeed;
    [SerializeField] GameObject pathToWhiskers;
    // use a LayerMask for Ghost to only detect Player in his line of sight
    [SerializeField] LayerMask lineOfSightLayerMask;

    [Header("Wall Check")]
    [SerializeField] Transform wallCheck;
    // use a LayerMask to filter by Ice layer to only return collisions with ice walls in the wall check
    [SerializeField] LayerMask wallLayerMask;

    private GameObject projectileSpawnPoint;
    private IEnumerator fireAtPlayer;
    private bool isFiring;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    // keep track of how many times the player hit Ghost
    public static int hitCount { get; private set; }
    // Ghost will be given 3 lives
    private const int livesCount = 3;
    // keep track of whether Ghost can be damaged
    private bool isInvincible = false;
    private BoxCollider2D boxCollider2D;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        // initialise variable to avoid null pointer
        hitCount = 0;
        // store the projectile spawn point's gameObject so we can access its position later
        projectileSpawnPoint = gameObject.transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (IsTouchingIceWall())
        {
            // flip the horizontal direction if Ghost bumps into ice wall
            transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
        }
    }

    private void FixedUpdate()
    {
       if (IsPlayerInSight())
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

    private bool IsTouchingIceWall()
    {
        bool isTouchIceWall = false;
        // determine if player is touching wall by checking colliders within a circlular area of Wall Check's transform.
        Collider2D collider = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayerMask);
        if (collider != null)
        {
            if (collider.tag == "Ice")
            {
                isTouchIceWall = true;
            }
        }
        return isTouchIceWall;
    }

    public void TakeDamage()
    {
        if (!isInvincible)
        {
            StartCoroutine(MakeInvincible());
            /* if Ghost gets hit 3 times, the platforms leading to whiskers show up
             * for the player to complete the level
             */
            hitCount += 1;

            // if Ghost has been defeated
            if (hitCount > livesCount - 1)
            {
                Destroy(gameObject, 0.1f);
                GameManager.Instance.isLevelCompleteRequirementMet = true;
                pathToWhiskers.SetActive(true);
            }
        } 
    }

    bool IsPlayerInSight()
    {
        /* draw ray from in front of Ghost and limit the ray's distance. 
         * The ray will only collide with objects in the Player layer since 
         * we only want the Ghost to be hostile when the player is in the Ghost's line of sight 
         */
        RaycastHit2D hit2d = Physics2D.Raycast(transform.position, -transform.right, raycastDistance, lineOfSightLayerMask);
        if (hit2d.collider !=  null)
        {
            Debug.DrawLine(transform.position, hit2d.point, Color.red);
            return true;
        }
        return false;
    }

    private IEnumerator MakeInvincible()
    {
        SetInvincibility(true);
        yield return new WaitForSeconds(0.1f);
        boxCollider2D.enabled = false;
        yield return new WaitForSeconds(2f);
        SetInvincibility(false);
        boxCollider2D.enabled = true;
    }

    private void SetInvincibility(bool state)
    {
        // change alpha to make it seem transparent as a visual indicator that it cannot take damge
        isInvincible = state;
        spriteRenderer.color = isInvincible ? new Color(255, 255, 255, 0.5f) : new Color(255, 255, 255, 1f);
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
}
