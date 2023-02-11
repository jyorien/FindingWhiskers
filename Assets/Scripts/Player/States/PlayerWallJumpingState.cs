using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This state happens when the player presses jump while in the Wall Sliding state.
/// It handles making the player jump diagionally upwards in the opposite direction that they are facing when sliding against a wall.
/// Player can only transition to Standing or Wall Sliding state from here.
/// </summary>
public class PlayerWallJumpingState : PlayerBaseState
{
    private float wallJumpingDirection;

    public override void EnterState(PlayerStateManager stateManager)
    {
        stateManager.animator.SetBool("Jumping", true);

        // store the direction opposite of where player is facing
        wallJumpingDirection = stateManager.isFacingRight ? -1 : 1;

        // if the player is not facing where they are supposed to, flip the GameObject horizontally
        if ((wallJumpingDirection == 1 && stateManager.transform.localRotation.y == -1) ||
            (wallJumpingDirection == -1 && stateManager.transform.localRotation.y == 0))
        {
            stateManager.FlipHorizontally();
        }

        // jump diagonally in the opposite direction that the player is facing, using the Wall Jumping Power defined in the ScriptableObject
        stateManager.rigidBody2D.velocity = new Vector2(wallJumpingDirection * stateManager.playerAttributes.wallJumpingPower.x, stateManager.playerAttributes.wallJumpingPower.y);
    }

    public override void UpdateState(PlayerStateManager stateManager)
    {
        // go back to Wall Sliding state if player touches the wall while wall jumping
        if (PlayerObstacleCollision.isTouchingWall)
        {
            stateManager.ChangeState(stateManager.wallSlidingState);
        }

        // go to Standing State if player touches the floor while wall jumping
        if (PlayerObstacleCollision.bottomColliderType == BottomColliderType.FLOOR)
        {
            stateManager.ChangeState(stateManager.standingState);
        }
    }

    public override void FixedUpdateState(PlayerStateManager stateManager)
    {

    }

    public override void ExitState(PlayerStateManager stateManager)
    {
        stateManager.animator.SetBool("Jumping", false);
    }
}
