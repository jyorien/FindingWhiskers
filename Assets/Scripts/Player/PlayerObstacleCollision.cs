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
    // to filter by Ground layer only while doing a dirt check
    [SerializeField] private LayerMask groundLayerMask;

    // stores the state of whether player is touching a wall so that the player states can have a reference
    public static bool isTouchingWall { get; private set; }
    // stores the type of collider the player is standing on
    public static BottomColliderType bottomColliderType { get; private set; }
    // stores the type of collider that last triggered the player's OnCollisionEnter2D method
    // so that Jumping State knows how to handle the player's x velocity in air
    public static GroundType lastTouchedGroundType { get; private set; }
    // stores the type of collider that is triggering the player's OnCollisionStay2D method
    // so that the Player States know how to handle the x velocity while on the floor
    public static GroundType currentGroundType { get; private set; }

    private void Awake()
    {
        attributesData.OnFrozenStateChanged.AddListener(SetIceCubeOverlayState);
    }

    private void Start()
    {
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        // get reference to the GameObject that shows an ice cube to indicate that the player is frozen
        iceCubeOverlay = gameObject.transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        // get a reference to the collider detected at the bottom of the player
        Collider2D bottomCollider = DetectBottomCollider();
        // set the bottomColliderType variable to the correct type based on the collider detected
        SetBottomColliderType(bottomCollider);

        // do a wall check every frame to update the static isTouchingWall variable
        // so that the player states in the Finite State Machine can have a reference to whether player is touching a wall
        CheckWall();

        // if player bumps into enemy on the sides of the player collider, player gets hit and loses a life
        Collider2D enemy = DetectEnemyCollider();
        if (enemy != null)
        {
            OnHit();
        }

        // since all the enemies implemet IDamageable, we just need to call TakeDamage() when the player bounces on top of any enemy
        // because the enemy behaviour for taking damage is handled in the separate enemy scripts
        if (bottomColliderType == BottomColliderType.ENEMY)
        {
            IDamageable damageable = bottomCollider.gameObject.GetComponentInParent<IDamageable>();
            damageable.TakeDamage();
        }
    }

    private void OnDestroy()
    {
        // stop subscribing to the events
        attributesData.OnFrozenStateChanged.RemoveListener(SetIceCubeOverlayState);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            // if player collides with an instant-death obstacle, restart level and lose a life
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "EndPole":
                // trigger event so LevelManager can display the Level Complete Scene and save the player's best timing if needed
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
    /// Set the Ice Cube Overlay's display state to show whether the player is frozen.
    /// </summary>
    /// <param name="isFrozen">Determines whether to show the Ice Cube Overlay.</param>
    private void SetIceCubeOverlayState(bool isFrozen)
    {
        iceCubeOverlay.SetActive(isFrozen);
    }

    /// <summary>
    /// Do a ground check from the middle of the player to the bottom.
    /// This ground check checks for objects in "Ground" Layer (Dirt, Ice) or if the player stepped on an enemy.
    /// </summary>
    private Collider2D DetectBottomCollider()
    {
        float heightOffset = 0.2f;

        // only allow 1 result to be returned by BoxCast
        RaycastHit2D[] raycastHits = new RaycastHit2D[1];

        // create a ContactFilter2D so that the BoxCast only detects objects on the 'Ground' and 'Enemy' layer
        ContactFilter2D contactFilter = CreateContactFilter2D(bottomCollisionLayerMask);

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

    /// <summary>
    /// Sets the bottomColliderType variable based on the given collider.
    /// </summary>
    /// <param name="bottomCollider">Determines the type that will be set to bottomColliderType.</param>
    private void SetBottomColliderType(Collider2D bottomCollider)
    {
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
    }

    /// <summary>
    /// Creates a ContactFilter2D that ignores trigger collisions and filters layers defined by the layerMask parameter.
    /// </summary>
    /// <param name="layerMask">The LayerMask to set in the ContactFilter2D.</param>
    /// <returns>Returns a ContactFilter2D that ignores trigger collisions and filters by the LayerMask given as parameter.</returns>
    private ContactFilter2D CreateContactFilter2D(LayerMask layerMask)
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(layerMask);
        contactFilter2D.useTriggers = false;
        return contactFilter2D;
    }

    /// <summary>
    /// Do a wall check by checking colliders within a circlular area of Wall Check's transform and filter by Ground layer and Ground tag.
    /// </summary>
    private void CheckWall()
    {
        isTouchingWall = false;
        Collider2D collider = Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayerMask);

        // we do not want player to wall slide or jump off ice or platforms, so make sure object in "Ground" layer is tagged "Ground" as all the dirt walls are tagged that
        if (collider && collider.tag == "Ground")
        {
            isTouchingWall = true;
        }
    }

    /// <summary>
    /// Handles the player's response to colliding with GameObjects that can defeat the player (enemies, instant-death obstacles, and Ghost's normal projectiles).
    /// The player's GameObject gets destroyed and they lose a life.
    /// </summary>
    private void OnHit()
    {
        Destroy(gameObject);
        livesManager.DecreaseLife();
    }

    /// <summary>
    /// Do an enemy check to check whether player collided with an enemy on the sides.
    /// </summary>
    /// <returns>Returns the enemy's Collider2D if an enemy was detected. Otherwise, it returns null.</returns>
    private Collider2D DetectEnemyCollider()
    {
        float widthOffset = 0.1f;
        // only allow 1 result to be returned by LineCast
        RaycastHit2D[] lineCastHits = new RaycastHit2D[1];

        // create a ContactFilter2D so that the LineCast only detects objects on the 'Enemy' layer
        ContactFilter2D contactFilter = CreateContactFilter2D(enemyLayerMask);

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
}