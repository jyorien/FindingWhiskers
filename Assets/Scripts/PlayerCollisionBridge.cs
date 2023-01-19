using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionBridge : MonoBehaviour
{
    [SerializeField] ColliderSide colliderSide;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        /* pass into PlayerMovement to handle collisions there
         pass colliderSide to identify which side the collider is at.
         colliderSide can be modified in the inspector */
        transform.parent.GetComponent<PlayerMovement>().OnCollisionDetected(colliderSide, collision);
    }
}
