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
            // flip the horizontal direction if Trunk bumps into walls or spikes
            transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
        }
    }

    void FixedUpdate()
    {
        // move forward in the direction Trunk is facing
        rigidBody2D.velocity = -transform.right * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            case "Wall":
            case "InstantDeath":
            case "Ice":
                // flip the horizontal direction if Trunk bumps into walls or spikes
                transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
                break;
        }
    }

    public void TakeDamage()
    {
        Destroy(gameObject, 0.1f);
    }

    /// <summary>
    /// Determine if player is touching wall by checking colliders within a circlular area of Wall Check GameObject's transform.
    /// </summary>
    /// <returns>Returns true if Trunk collides with a wall infront</returns>
    private bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayerMask);
    }
}
