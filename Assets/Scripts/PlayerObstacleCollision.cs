using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerObstacleCollision : MonoBehaviour
{
    [SerializeField] BoxCollider2D playerCollider;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            // if player touches an instant-death obstacle, restart level
            case "InstantDeath":
                GameManager.Instance.OnGameLose();
                break;
        }
        
    }

    // detect collision without colliding with Rigidbody2D
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "EndPole":
                GameManager.Instance.OnGameWin();
                break;
        }
    }
}

