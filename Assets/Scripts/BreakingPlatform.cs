using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingPlatform : MonoBehaviour
{
    BoxCollider2D platformCollider;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        platformCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.collider.tag)
        {
            case "Player":
                Debug.Log("start breaking");
                //  break platform after a second and respawn after two seconds
                StartCoroutine(BreakPlatform());
                break;
        }
    }

    IEnumerator BreakPlatform()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("break");
        SetPlatformActive(false);
        yield return new WaitForSeconds(2);
        SetPlatformActive(true);
    }

    void SetPlatformActive(bool isActive)
    {
        platformCollider.enabled = isActive;
        spriteRenderer.enabled = isActive;
    }
}
