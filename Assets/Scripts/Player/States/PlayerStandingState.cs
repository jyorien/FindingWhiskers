using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the default state when the player is not pressing any button.
/// Player can only transition to Walking or Jumping state from here.
/// </summary>
public class PlayerStandingState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager stateManager)
    {
        stateManager.animator.SetBool("Standing", true);
        // when player is not pressing any movement buttons, reset the extra speed added for the acceleration on ice
        stateManager.playerAttributes.currentSpeedOnIce = 0;
    }

    public override void UpdateState(PlayerStateManager stateManager)
    {
        // if player presses a movement key, change to Walking state
        if (stateManager.horizontalMovement != 0)
        {
            stateManager.ChangeState(stateManager.walkingState);
        }

        // if player is grounded and presses on the Jump button, or when player is not standing on the floor, change to Jumping state
        if ((stateManager.isJumpButtonPressed &&
            PlayerObstacleCollision.bottomColliderType == BottomColliderType.FLOOR) ||
            PlayerObstacleCollision.bottomColliderType == BottomColliderType.NONE
            )
        {
            stateManager.ChangeState(stateManager.jumpingState);
        }
    }

    public override void FixedUpdateState(PlayerStateManager stateManager)
    {
        // if player on dirt, set velocity to zero. if on ice, keep velocity and let the Ice material's friction slow player down
        if (PlayerObstacleCollision.currentGroundType == GroundType.DIRT)
        {
            stateManager.rigidBody2D.velocity = new Vector2(0, stateManager.rigidBody2D.velocity.y);
        }

    }

    public override void ExitState(PlayerStateManager stateManager)
    {
        stateManager.animator.SetBool("Standing", false);
    }
}
