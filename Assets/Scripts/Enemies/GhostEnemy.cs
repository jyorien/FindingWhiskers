using System.Collections;
using UnityEngine;

public class GhostEnemy : MonoBehaviour, IDamageable
{
    [Header("Ghost Attributes")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float raycastDistance;
    // use a LayerMask for Ghost to only detect Player in his line of sight
    [SerializeField] private LayerMask lineOfSightLayerMask;

    [Header("Projectiles")]
    [SerializeField] private GameObject ghostProjectileToSpawn;
    [SerializeField] private GameObject iceCubeProjectileToSpawn;
    [SerializeField] private float projectileSpeed;
    // keep a reference to the coroutine that continuously fires projectiles so we can start and stop the same instance
    private IEnumerator fireAtPlayer;
    private bool isFiring;

    [Header("Wall Check")]
    [SerializeField] private Transform wallCheck;
    // use a LayerMask to filter by Ice layer to only return collisions with ice walls in the wall check
    [SerializeField] private LayerMask wallLayerMask;

    [Header("Scriptable Objects")]
    // manage Ghost's lives in a ScriptableObject
    [SerializeField] private LivesManagerSO ghostLivesManager;
    // get reference to Level Data so Ghost can update the Level Complete Requirement status
    [SerializeField] private LevelDataSO levelData;

    // component references
    private Rigidbody2D rigidBody2D;
    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;
    private GameObject projectileSpawnPoint;

    // keep track of whether Ghost can be damaged
    // if Ghost can be damaged, isInvincible is false, otherwise it is true
    private bool isInvincible = false;

    // Start is called before the first frame update
    private void Start()
    {
        // subscribe to changes in Ghost's lives
        ghostLivesManager.OnLivesChanged.AddListener(HandleLivesChanged);
        ghostLivesManager.ResetLives();

        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        // store the Projectile Spawn Point's GameObject so we can access its position later
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
            rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);
            // if already firing, don't start the coroutine
            if (isFiring) return;

            // start firing at player when Ghost sees player
            fireAtPlayer = FireAtPlayer();
            StartCoroutine(fireAtPlayer);

            // flag that Ghost is already firing so we don't start the coroutine again
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
            // move forward in the direction Ghost is facing
            rigidBody2D.velocity = -transform.right * moveSpeed;
        }
    }

    private void OnDestroy()
    {
        // stop subscribing to the event
        ghostLivesManager.OnLivesChanged.RemoveListener(HandleLivesChanged);
    }

    public void TakeDamage()
    {
        // make Ghost invincible for a few seconds when hit so that player does not continuously bounce on Ghost to win
        // update the ScriptableObject for the UI to update and for the Path to Whiskers platform to know whether to enable itself
        if (!isInvincible)
        {
            StartCoroutine(MakeInvincible());
            ghostLivesManager.DecreaseLife();
        }
    }

    /// <summary>
    /// Determine if player is touching an ice wall by checking colliders within a circlular area of the Wall Check GameObject's transform and checking if the tag is "Ice".
    /// </summary>
    /// <returns>Returns true if Trunk collides with an ice wall in front.</returns>
    private bool IsTouchingIceWall()
    {
        bool isTouchIceWall = false;
        Collider2D collider = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayerMask);

        if (collider != null)
        {
            if (collider.CompareTag("Ice"))
            {
                isTouchIceWall = true;
            }
        }
        return isTouchIceWall;
    }

    /// <summary>
    /// Determine if player is in front of Ghost by casting a ray with a fixed distance in the direction Ghost is facing.
    /// </summary>
    /// <returns>Returns whether player is in Ghost's line of sight.</returns>
    private bool IsPlayerInSight()
    {
        // the ray will only collide with objects in the Player layer since 
        // we only want the Ghost to be hostile when the player is in the Ghost's line of sight 
        RaycastHit2D hit2d = Physics2D.Raycast(transform.position, -transform.right, raycastDistance, lineOfSightLayerMask);
        if (hit2d.collider != null)
        {
            // if hit, draw a red line between Ghost and the player for debugging purposes
            Debug.DrawLine(transform.position, hit2d.point, Color.red);
            return true;
        }
        return false;
    }

    private void HandleLivesChanged(int livesLeft)
    {
        // if Ghost has been defeated
        if (livesLeft < 1)
        {
            // destroy Ghost's GameObject and flag the Level Complete Requirement as true so other listeners can handle the change
            Destroy(gameObject, 0.1f);
            levelData.isLevelCompleteRequirementMet = true;
        }
    }

    /// <summary>
    /// Temporarily make Ghost invincible by disabling the collider to prevent Ghost from detecting collisions and taking damage.
    /// </summary>
    private IEnumerator MakeInvincible()
    {
        SetInvincibility(true);
        // set a short delay to ensure player bounces on Ghost before disabling the collider
        yield return new WaitForSeconds(0.1f);
        boxCollider2D.enabled = false;
        yield return new WaitForSeconds(2f);
        SetInvincibility(false);
        boxCollider2D.enabled = true;
    }

    /// <summary>
    /// This updates the Ghost invincibility state. It also updates the sprite's alpha as feedback to show whether Ghost can be damaged.
    /// </summary>
    /// <param name="state">Determines whether Ghost is invincible.</param>
    private void SetInvincibility(bool state)
    {
        // change alpha to make it seem transparent as a visual indicator that it cannot take damage
        isInvincible = state;
        spriteRenderer.color = isInvincible ? new Color(255, 255, 255, 0.5f) : new Color(255, 255, 255, 1f);
    }

    /// <summary>
    /// Handles creating and destroying a new projectile that spawns from the Ghost's Projectile Spawn Point.
    /// </summary>
    private void FireProjectile()
    {
        // get the location to spawn the projectile
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

        Rigidbody2D projectileRb = newProjectile.GetComponent<Rigidbody2D>();

        // add force to shoot projectile in the direction Ghost is facing
        projectileRb.AddForce(-transform.right * projectileSpeed);
        
        // destroy projectile after a few seconds to avoid keeping too many unused GameObjects
        Destroy(newProjectile, 3);
    }

    /// <summary>
    /// Continuously fires a new projectile after every delay
    /// </summary>
    private IEnumerator FireAtPlayer()
    {
        // add a short delay before the loop so when player jumps to re-enter the line of sight,
        // Ghost doesn't immediately start firing
        yield return new WaitForSeconds(0.5f);
        // keep firing infinitely with a set delay
        while (true)
        {
            // randomly spawn normal or ice cube projectile every 1.5s
            FireProjectile();
            yield return new WaitForSeconds(1.5f);
        }
    }
}
