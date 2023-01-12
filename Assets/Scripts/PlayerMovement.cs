using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float horizontalMovement;
    bool isJump;
    int jumpCount = 0;
    [SerializeField] float jumpForce;
    [SerializeField] float speed;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D playerCollider;


    // Update is called once per frame
    void Update()
    {
        // get movement input to detect which direction player wants to move
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        isJump = Input.GetButtonDown("Jump");
        if (isJump && jumpCount < 2)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount += 1;
        }
    }

    private void FixedUpdate()
    {
        // move player horizontally based on speed in inspector
        rb.velocity = new Vector2(horizontalMovement * speed, rb.velocity.y);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            case "Ground":
                jumpCount = 0;
                break;

            case "InstantDeath":
                Debug.Log("Player dies");
                break;
        }
        
    }
}
