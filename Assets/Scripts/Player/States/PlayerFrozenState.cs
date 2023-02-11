using System.Collections;
using UnityEngine;

/// <summary>
/// This state happens when Ghost freezes the player. The player will not be allowed to move at all.
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
        // no code to handle inputs as player cannot move when frozen
    }

    public override void FixedUpdateState(PlayerStateManager stateManager)
    {
        // no code to handle inputs as player cannot move when frozen
    }

    public override void ExitState(PlayerStateManager stateManager)
    {
        stateManager.animator.SetBool("Standing", false);
    }

    /// <summary>
    /// Starts the freeze timer and changes the frozen state to false when the timer is up.
    /// It automatically changes to Standing State when timer is up.
    /// </summary>
    public IEnumerator Freeze(PlayerStateManager stateManager)
    {
        yield return new WaitForSeconds(2);
        stateManager.playerAttributes.ChangeFrozenState(false);
        stateManager.ChangeState(stateManager.standingState);
    }
}
