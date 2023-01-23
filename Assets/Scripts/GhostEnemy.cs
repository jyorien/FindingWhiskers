using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    [SerializeField] float raycastDistance;
    [SerializeField] GameObject projectileToSpawn;
    [SerializeField] float projectileSpeed;

    private GameObject projectileSpawnPoint;
    private int layerMask;
    private IEnumerator fireAtPlayer;
    private bool isFiring;
    // Start is called before the first frame update
    void Start()
    {
        // use a LayerMask for Ghost to only detect Player in his line of sight
        layerMask = 1 << LayerMask.NameToLayer("Player");
        // store the projectile spawn point's gameObject so we can access its position later
        projectileSpawnPoint = gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
       if (isPlayerInSight())
        {
            // if already firing, don't start the coroutine
            if (isFiring) return;

            // start firing at player when Ghost sees player
            fireAtPlayer = FireAtPlayer();
            StartCoroutine(fireAtPlayer);

            // flag that Ghost is already firing
            isFiring = true;
        }
        else
        {
            // if Ghost was firing, stop firing since player is out of sight
            if (isFiring)
            {
                StopCoroutine(fireAtPlayer);
                isFiring = false;
            }
                
        }
    }

    bool isPlayerInSight()
    {
        /* draw ray from Ghost towards the right and limit the ray's distance. 
         * The ray will only collide with objects in the Player layer since we only want the Ghost to be hostile 
         * when the player is in the Ghost's line of sight 
         */
        RaycastHit2D hit2d = Physics2D.Raycast(transform.position, -transform.right, raycastDistance, layerMask);
        if (hit2d.collider !=  null)
        {
            Debug.DrawLine(transform.position, hit2d.point, Color.red);
            return true;
        }
        return false;
    }

    void FireProjectile()
    {
        // get location to spawn projectile
        Vector3 projectileSpawnPointLocation = projectileSpawnPoint.transform.position;

        // create the new projectile
        GameObject newProjectile = Instantiate(projectileToSpawn, projectileSpawnPointLocation, Quaternion.identity);

        // add force to push projectile forward
        Rigidbody2D projectileRb = newProjectile.GetComponent<Rigidbody2D>();
        projectileRb.AddForce(Vector2.left * projectileSpeed);

        // destroy projectile after a few seconds to avoid keeping too many unused GameObjects
        Destroy(newProjectile, 3);
    }

    IEnumerator FireAtPlayer()
    {
        // keep firing infinitely with a set delay
        while (true)
        {
            FireProjectile();
            yield return new WaitForSeconds(1.5f);
        }
    }
}
