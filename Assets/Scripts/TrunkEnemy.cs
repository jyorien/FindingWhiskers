using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkEnemy : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform playerCheck;
    [SerializeField] LayerMask playerLayerMask;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;

    // Determines if Trunk can turn his transform upon colliding with a wall
    private bool canTurn = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isPlayerOnTop())
        {
            // ensure player bounces before destroying gameObject
            // set a delay to ensure that player touches the Trunk's collider with Bouncy PhysicsMaterial
            Destroy(gameObject, 0.1f);
        }
    }

    void FixedUpdate()
    {
  
        rb.velocity = -transform.right * speed;
    }

    private bool isPlayerOnTop()
    {
        bool isOnTop = false;

        Collider2D collider = Physics2D.OverlapBox(playerCheck.position, new Vector2(2f, 0.1f), 0, playerLayerMask);
        if (collider != null)
        {
            Vector3 extentsOfTrunk = boxCollider2D.bounds.extents;
            Vector3 centerOfTrunk = boxCollider2D.bounds.center;
            Vector3 positionOfIncomingColider = collider.transform.position;

            // if incoming collider touches the top of Trunk and is tagged Player, then player is on top
            if (positionOfIncomingColider.y >= centerOfTrunk.y + extentsOfTrunk.y && collider.tag == "Player")
            {
                isOnTop = true;

            }
        }
        return isOnTop;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(playerCheck.position,new Vector3(2f,0.1f,0));
    }

    IEnumerator TurningCooldown()
    {
        /* when Trunk turns, it might trigger another OnCollisionEnter2D event which might cause Trunk to turn endlessly
         * set a cooldown period so Trunk can move away from the wall before turning again
         */
        canTurn = false;
        yield return new WaitForSeconds(0.5f);
        canTurn = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            case "Player":
                Vector3 extentsOfTrunk = boxCollider2D.bounds.extents;
                Vector3 centerOfTrunk = boxCollider2D.bounds.center;
                Vector3 positionOfIncomingColider = collision.collider.transform.position;

                /* if incoming collider touches the left and right edges of Trunk, player loses
                 * extents returns half the size of the collider.
                 * we are checking if player's x point is less than center position - half the length of Trunk
                 * or if x point is greater than center position + half the length of Trunk
                 */
                if (positionOfIncomingColider.x > centerOfTrunk.x + extentsOfTrunk.x ||
                    positionOfIncomingColider.x < centerOfTrunk.x - extentsOfTrunk.x)
                {
                    Destroy(collision.gameObject);
                    GameManager.Instance.OnGameLose();

                }
                break;

            case "Wall":
            case "InstantDeath":
            case "Ice":
                if (canTurn)
                {
                    // flip the horizontal direction if Trunk bumps into wall or spikes
                    transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
                    StartCoroutine(TurningCooldown());
                }
                StartCoroutine(TurningCooldown());
                break;
        }
    }
}
