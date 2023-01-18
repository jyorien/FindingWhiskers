using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerObstacleCollision : MonoBehaviour
{
    [SerializeField] BoxCollider2D playerCollider;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerMovement movement;
    [SerializeField] float slipperyForce;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            // if player touches an instant-death obstacle, restart level
            case "InstantDeath":
                GameManager.Instance.OnGameLose();
                break;
            // if player touches an enemy, the enemy gets destroyed and the player bounces off
            // TODO: only defeat enemy if collide on top, otherwise player loses by touching enemy
            case "Enemy":
                Destroy(collision.gameObject);
                Debug.Log("enemy");
                break;
        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            // add constant force to player if he stands on ice to make the surface seem slippery
            case "Ice":
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
                break;

            case "Clue":
                GameManager.Instance.OnClueFound();
                Destroy(collision.gameObject);
                break;
        }
    }
}

