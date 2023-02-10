using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This state happens when Ghost freezes the player.
/// Player transitions to Standing state after the freeze timer.
/// </summary>
public class PlayerFrozenState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager stateManager)
    {
        stateManager.animator.SetBool("Standing", true);

        // reset the extra speed added for the acceleration on ice
        stateManager.playerAttributes.currentSpeedOnIce = 0;
        stateManager.StartCoroutine(Freeze(stateManager));
    }

    public override void UpdateState(PlayerStateManager stateManager)
    {
    }

    public override void FixedUpdateState(PlayerStateManager stateManager)
    {

    }

    public override void ExitState(PlayerStateManager stateManager)
    {
        stateManager.animator.SetBool("Standing", false);
    }

    public IEnumerator Freeze(PlayerStateManager stateManager)
    {
        yield return new WaitForSeconds(2);
        stateManager.playerAttributes.ChangeFrozenState(false);
        stateManager.ChangeState(stateManager.standingState);
    }
}
