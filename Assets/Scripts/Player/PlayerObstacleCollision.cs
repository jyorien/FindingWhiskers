using System.Collections;
using UnityEngine;

public enum GroundType { NONE, DIRT, ICE }
public enum BottomColliderType { NONE, FLOOR, ENEMY }

public class PlayerObstacleCollision : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;
    [SerializeField] private PlayerMovement movement;

    // to filter what player can detect from the ground check
    [SerializeField] private LayerMask bottomCollisionLayerMask;
    // to filter enemies when detecting for enemies
    [SerializeField] private LayerMask enemyLayerMask;

    public static BottomColliderType bottomColliderType { get; private set; }
    public static GroundType groundType { get; private set; }

    private GameObject iceCubeOverlay;


    // keep track of whether player is frozen to activate/deactivate certain behaviours
    private bool isFrozen = false;

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

        // if player bumps into enemy on the sides, player loses
        Collider2D enemy = DetectEnemyCollider();
        if (enemy != null)
        {
            Destroy(gameObject);
            GameManager.Instance.OnGameLose();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            // if player collides with an instant-death obstacle, restart level
            case "InstantDeath":
                Destroy(gameObject);
                GameManager.Instance.OnGameLose();
                break;
            case "Ice":
                groundType = GroundType.ICE;
                break;
            case "Ground":
            case "Platform":
                groundType = GroundType.DIRT;
                break;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            // don't apply force when player is frozen
            case "Ice":
                if (isFrozen) return;
                break;
        }
    }

    // detect collision without colliding with Rigidbody2D
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "EndPole":
                // only complete level if player collected clue / defeated boss
                if (GameManager.Instance.isLevelCompleteRequirementMet)
                {
                    GameManager.Instance.OnGameWin();
                    // disable player from moving when they finish the level
                    movement.canMove = false;
                }
                break;

            case "Clue":
                // flag as clue collected
                GameManager.Instance.isLevelCompleteRequirementMet = true;
                // destroy the GameObject since we don't need it anymore
                Destroy(collision.gameObject);
                break;

            case "Campfire":
                // player regains max attributes
                movement.ResetToMaxAttributeValues();
                break;

            case "InstantDeath":
                // only projectiles have InstantDeath tag as trigger, hence destroy projectile on touch
                Destroy(collision.gameObject);
                /* when frozen, don't allow player to lose from touching ghost projectiles
                 * as it might be unfair
                 */
                if (isFrozen) return;
                Destroy(gameObject);
                GameManager.Instance.OnGameLose();
                break;
            case "Freeze":
                // destroy ice cube projectile on touch
                Destroy(collision.gameObject);
                // don't allow the freezing effect to stack
                if (isFrozen) return;
                StartCoroutine(FreezePlayer());
                break;
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

    private IEnumerator FreezePlayer()
    {
        SetFreeze(true);
        yield return new WaitForSeconds(2);
        SetFreeze(false);
    }

    private void SetFreeze(bool isFreeze)
    {
        // update frozen state
        isFrozen = isFreeze;
        movement.canMove = !isFreeze;
        iceCubeOverlay.SetActive(isFreeze);
    }
}

