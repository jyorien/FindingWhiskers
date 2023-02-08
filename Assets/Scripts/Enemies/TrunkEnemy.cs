using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private LayerMask wallLayerMask;

    private Rigidbody2D rigidBody2D;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsTouchingWall())
        {
            // flip the horizontal direction if Trunk bumps into wall or spikes
            transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
        }
    }

    void FixedUpdate()
    {
        rigidBody2D.velocity = -transform.right * speed;
    }

    public void TakeDamage()
    {
        Destroy(gameObject, 0.1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(playerCheck.position,new Vector3(2f,0.1f,0));
    }

    private bool IsTouchingWall()
    {
        // determine if player is touching wall by checking colliders within a circlular area of Wall Check's transform.
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayerMask);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            case "Wall":
            case "InstantDeath":
            case "Ice":
                // flip the horizontal direction if Trunk bumps into wall or spikes
                transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
                break;
        }
    }
}
