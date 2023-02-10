using UnityEngine;

/// <summary>
/// Handles the player's collision detection and response to the game's obstacles
/// </summary>
public class PlayerObstacleCollision : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;
    private GameObject iceCubeOverlay;

    [SerializeField] private PlayerAttributesManager attributesManager;

    [Header("Scriptable Objects")]
    [SerializeField] private PlayerAttributesDataSO attributesData;
    [SerializeField] private LivesManagerSO livesManager;
    [SerializeField] private LevelDataSO levelData;

    [Header("Components")]
    [SerializeField] private Transform wallCheck;

    [Header("Layer Masks")]
    // to filter what player can detect from the ground check
    [SerializeField] private LayerMask bottomCollisionLayerMask;
    // to filter enemies when detecting for enemies
    [SerializeField] private LayerMask enemyLayerMask;
    // to filter by Ground only while doing a dirt check
    [SerializeField] private LayerMask groundLayerMask;

    public static bool isTouchingWall { get; private set; }
    // stores the type of collider the player is standing on
    public static BottomColliderType bottomColliderType { get; private set; }
    // stores the type of collider that last triggered the player's OnCollisionEnter2D method
    // so that Jumping State knows how to handle the player's x velocity in air
    public static GroundType lastTouchedGroundType { get; private set; }
    // stores the type of collider that is triggering the player's OnCollisionStay2D method
    // so that the Player States know how to handle the x velocity
    public static GroundType currentGroundType { get; private set; }

    private void Awake()
    {
        attributesData.OnFrozenStateChanged.AddListener(SetIceCubeOverlayState);
    }

    private void Start()
    {
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        // get reference to the GameObject that shows Robin frozen
        iceCubeOverlay = gameObject.transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        Collider2D bottomCollider = DetectBottomCollider();
        if (bottomCollider != null)
        {
            switch (bottomCollider.tag)
            {
                case "Ground":
                case "Platform":
                case "Ice":
                    bottomColliderType = BottomColliderType.FLOOR;
                    break;
                case "Enemy":
                    bottomColliderType = BottomColliderType.ENEMY;
                    break;
                default:
                    bottomColliderType = BottomColliderType.NONE;
                    break;
            }
        }
        else
        {
            bottomColliderType = BottomColliderType.NONE;
        }

        CheckWall();

        // if player bumps into enemy on the sides, player loses
        Collider2D enemy = DetectEnemyCollider();
        if (enemy != null)
        {
            OnHit();
        }

        /* since all the enemies implemet IDamageable, we just need to call TakeDamage() when the player bounces on top of any enemy
         * because the enemy behaviour for taking damage is handled in the separate enemy scrips
         */
        if (bottomColliderType == BottomColliderType.ENEMY)
        {
            IDamageable damageable = bottomCollider.gameObject.GetComponentInParent<IDamageable>();
            damageable.TakeDamage();
        }
    }

    private void OnDestroy()
    {
        attributesData.OnFrozenStateChanged.RemoveListener(SetIceCubeOverlayState);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            // if player collides with an instant-death obstacle, restart level
            case "InstantDeath":
                OnHit();
                break;
            case "Ice":
                lastTouchedGroundType = GroundType.ICE;
                break;
            case "Ground":
            case "Platform":
                lastTouchedGroundType = GroundType.DIRT;
                break;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            case "Ice":
                currentGroundType = GroundType.ICE;
                break;
            case "Ground":
            case "Platform":
                currentGroundType = GroundType.DIRT;
                break;
            default:
                currentGroundType = GroundType.NONE;
                break;
        }
    }

    // detect collision without colliding with Rigidbody2D
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "EndPole":
                // disable player from moving when they finish the level
                levelData.TriggerOnWin();
                break;

            case "Clue":
                // flag as clue collected
                levelData.isLevelCompleteRequirementMet = true;
                // destroy the GameObject since we don't need it anymore
                Destroy(collision.gameObject);
                break;

            case "Campfire":
                // player regains max attributes
                attributesManager.ResetToMaxAttributeValues();
                break;

            case "InstantDeath":
                // only projectiles have InstantDeath tag as trigger, hence destroy projectile on touch
                Destroy(collision.gameObject);

                // when frozen, don't allow player to lose from touching ghost projectiles as it might be unfair
                if (attributesData.isFrozen) return;
                OnHit();
                break;

            case "Freeze":
                // destroy ice cube projectile on touch
                Destroy(collision.gameObject);

                // don't allow the freezing effect to stack
                if (attributesData.isFrozen) return;
                attributesData.ChangeFrozenState(true);
                break;
        }
    }

    /// <summary>
    /// Use raycasting to do a ground check from the middle of the player to the bottom and filter by "Ground" tag to only return true
    /// if a Dirt Floor or Dirt Wall was detected.
    /// </summary>
    public bool IsOnDirt()
    {
        // add extra height to detect a bit below the player
        float heightOffset = 0.2f;

        // only allow 1 result to be returned by BoxCast
        RaycastHit2D[] raycastHits = new RaycastHit2D[1];

        /* create a ContactFilter2D so that the BoxCast only detects objects on the 'Ground' layer
         * and ignore trigger collisions so the player cannot jump when touching platforms that are in isTrigger
         */
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(groundLayerMask);
        contactFilter.useTriggers = false;

        Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, contactFilter, raycastHits, heightOffset);
        // store the only result returned into a variable for easy reference
        RaycastHit2D raycastHit2D = raycastHits[0];

        // to avoid NullPointerException, check whether collider is null first
        if (raycastHit2D.collider != null)
        {
            return raycastHit2D.collider.tag == "Ground" || raycastHit2D.collider.tag == "Platform";
        }
        else
        {
            // if null, immediately return false as player is definitely not on dirt
            return false;
        }
    }

    private void CheckWall()
    {
        isTouchingWall = false;
        /* determine if player is touching wall by checking colliders within a circlular area of Wall Check's transform.
         * detects walls based on their layer
         */
        Collider2D collider = Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayerMask);

        // We do not want player to wall slide or jump off ice or platforms, so make sure object in "Ground" layer is labelled "Ground" as all the dirt walls are tagged that
        if (collider && collider.tag == "Ground")
        {
            isTouchingWall = true;
        }
    }

    private Collider2D DetectBottomCollider()
    {
        // use raycasting to do a ground check from the middle of the player to the bottom
        // note: ground check checks for objects in "Ground" Layer (Dirt Floor, Ice)

        float heightOffset = 0.2f;

        // only allow 1 result to be returned by BoxCast
        RaycastHit2D[] raycastHits = new RaycastHit2D[1];

        /* create a ContactFilter2D so that the BoxCast only detects objects on the 'Ground' layer
         * and ignore trigger collisions so the player cannot jump when touching platforms that are in isTrigger
         */
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(bottomCollisionLayerMask);
        contactFilter.useTriggers = false;

        // draw a box below the player with a small height offset to detect incoming colliders with the ground or enemies
        Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, contactFilter, raycastHits, heightOffset);

        // store the only result returned into a variable for easy reference
        RaycastHit2D raycastHit2D = raycastHits[0];

        Color rayColor = raycastHit2D.collider != null ? Color.red : Color.white;

        // draw where the BoxCast hits for debugging purposes
        Debug.DrawRay(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + heightOffset), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + heightOffset), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.y + heightOffset), Vector2.right * boxCollider2D.bounds.extents.x * 2, rayColor);

        if (raycastHit2D.collider != null)
        {
            return raycastHit2D.collider;
        }
        return null;
    }

    private Collider2D DetectEnemyCollider()
    {
        float widthOffset = 0.1f;
        // only allow 1 result to be returned by LineCast
        RaycastHit2D[] lineCastHits = new RaycastHit2D[1];

        /* create a ContactFilter2D so that the LineCast only detects objects on the 'Enemy' layer
         * and ignore trigger collisions so the player cannot jump when touching platforms that are in isTrigger
         */
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(enemyLayerMask);
        contactFilter.useTriggers = false;

        // draw a horizontal line across the player with a small width offset to detect incoming collisions with enemies
        Physics2D.Linecast(new Vector3(boxCollider2D.bounds.center.x - boxCollider2D.bounds.extents.x - widthOffset, boxCollider2D.bounds.center.y, 0),
            new Vector3(boxCollider2D.bounds.center.x + boxCollider2D.bounds.extents.x + widthOffset, boxCollider2D.bounds.center.y, 0), contactFilter, lineCastHits);

        // store the only result returned into a variable for easy reference
        RaycastHit2D lineCastHit = lineCastHits[0];

        // draw where LineCast hits for debugging purposes
        Debug.DrawLine(new Vector3(boxCollider2D.bounds.center.x - boxCollider2D.bounds.extents.x - widthOffset, boxCollider2D.bounds.center.y, 0),
            new Vector3(boxCollider2D.bounds.center.x + boxCollider2D.bounds.extents.x + widthOffset, boxCollider2D.bounds.center.y, 0), Color.red);

        if (lineCastHit.collider != null)
        {
            return lineCastHit.collider;
        }
        return null;
    }

    private void SetIceCubeOverlayState(bool isFrozen)
    {
       
       iceCubeOverlay.SetActive(isFrozen);
    }

    private void OnHit() {
        Destroy(gameObject);
        livesManager.DecreaseLife();
    }
}

