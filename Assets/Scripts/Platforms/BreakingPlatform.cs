using System.Collections;
using UnityEngine;

public class BreakingPlatform : Platform
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
    }

    protected override void OnPlayerStandingOnPlatform(Collider2D collision)
    {
        // start timer to break platform when player is on it
        StartCoroutine(BreakPlatform());
    }

    protected override void OnPlayerLeavesPlatform(Collision2D collision)
    {

    }

    /// <summary>
    /// Breaks and respawns the platform after a delay.
    /// </summary>
    private IEnumerator BreakPlatform()
    {
        animator.SetBool("Breaking", true);
        // break platform after one second
        yield return new WaitForSeconds(1);
        SetPlatformActive(false);

        // respawn after two seconds
        yield return new WaitForSeconds(2);
        animator.SetBool("Breaking", false);
        SetPlatformActive(true);
    }

    /// <summary> 
    /// Set the state of collider and sprite to give platform's "breaking" effect.
    /// </summary>
    /// <param name="isActive">Determines whether to enable or disable the collider and sprite.</param>
    private void SetPlatformActive(bool isActive)
    {
        platformCollider.enabled = isActive;
        spriteRenderer.enabled = isActive;
    }
}
