using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Axis axis;
    [SerializeField] float speed;
    bool isReverse = false;

    private void Update()
    {
        float velocity = isReverse ? speed * -1 : speed;
        switch (axis)
        {
            case Axis.Horizontal:
                transform.Translate(new Vector3(velocity * Time.deltaTime, 0, 0));

                break;
            case Axis.Vertical:
                transform.Translate(new Vector3(0, velocity * Time.deltaTime, 0));
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Debug.Log(name);
        switch (collision.collider.tag)
        {
            case "Player":
                collision.collider.transform.SetParent(transform);
                break;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            case "Player":
                collision.collider.transform.SetParent(null);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string collisionName = collision.name;
        if (collisionName == "Start Point" || collisionName == "End Point")
        {
            Debug.Log($"Collided with {collisionName}");
            isReverse = !isReverse;
            Debug.Log(isReverse);
        }
    }

}

enum Axis
{
    Horizontal,
    Vertical
}
