using System.Collections;
using UnityEngine;

public class PlayerObstacleCollision : MonoBehaviour
{
    [SerializeField] BoxCollider2D playerCollider;
    [SerializeField] PlayerMovement movement;
    [SerializeField] float slipperyForce;

    private GameObject iceCubeOverlay;
    private Rigidbody2D rb;

    // keep track of whether player is frozen to activate/deactivate certain behaviours
    private bool isFrozen = false;

    private void Start()
    {
        // get reference to the GameObject that shows Robin frozen
        iceCubeOverlay = gameObject.transform.GetChild(2).gameObject;
        rb = gameObject.GetComponent<Rigidbody2D>();
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
        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            // add constant force to player if he stands on ice to make the surface seem slippery
            // don't apply force when player is frozen
            case "Ice":
                if (isFrozen) return;
                Vector3 direction = movement.isFacingRight ? Vector3.right : Vector3.left;
                rb.AddForce(direction * slipperyForce);
                break;
        }
    }

    // detect collision without colliding with Rigidbody2D
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "EndPole":
                GameManager.Instance.OnFinishPoleTouched();
                // disable player from moving when they finish the level
                movement.canMove = false;
                break;

            case "Clue":
                GameManager.Instance.OnClueFound();
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

    IEnumerator FreezePlayer()
    {
        SetFreeze(true);
        yield return new WaitForSeconds(2);
        SetFreeze(false);
    }

    void SetFreeze(bool isFreeze)
    {
        // update frozen state
        isFrozen = isFreeze;
        movement.canMove = !isFreeze;
        iceCubeOverlay.SetActive(isFreeze);
    }
}

