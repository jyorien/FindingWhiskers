using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCollisionBridge : MonoBehaviour
{
    [SerializeField] ColliderSide colliderSide;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        /* pass into GhostEnemy to handle collisions there
         pass colliderSide to identify which side the collider is at.
         colliderSide can be modified in the inspector */
        transform.parent.GetComponent<GhostEnemy>().OnChildCollisionDetected(colliderSide, collision);
    }
}
