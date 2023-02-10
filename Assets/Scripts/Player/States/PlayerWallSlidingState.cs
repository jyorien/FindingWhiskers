using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This state happens when the player jumps or walks into a wall. It handles making the player slowly slide down the wall
/// as long as they are pressing a horizontal movement key against a wall.
/// Player can only transition to Standing, Walking, Jumping or Wall Jumping state from here.
/// </summary>
public class PlayerWallSlidingState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager stateManager)
    {
        stateManager.animator.SetBool("Wall Sliding", true);
    }

    public override void UpdateState(PlayerStateManager stateManager)
    {
        if (stateManager.isJumpButtonPressed)
        {
            
            stateManager.ChangeState(stateManager.wallJumpingState);
        }
    }

    public override void FixedUpdateState(PlayerStateManager stateManager)
    {
        if (PlayerObstacleCollision.bottomColliderType == BottomColliderType.FLOOR ||
            (stateManager.horizontalMovement > 0 && !stateManager.isFacingRight) ||
             (stateManager.horizontalMovement < 0 && stateManager.isFacingRight))
        {
            stateManager.ChangeState(stateManager.horizontalMovement == 0 ? stateManager.standingState : stateManager.walkingState);
        }

        // check if player is touching a wall while in air
        if (PlayerObstacleCollision.isTouchingWall &&
            PlayerObstacleCollision.bottomColliderType != BottomColliderType.FLOOR)
        {
            // allow player to slide down walls slowly if they are moving in the direction they are facing
            if (stateManager.horizontalMovement > 0 && stateManager.isFacingRight ||
                stateManager.horizontalMovement < 0 && !stateManager.isFacingRight)
            {
                stateManager.rigidBody2D.velocity = new Vector2(stateManager.rigidBody2D.velocity.x, Mathf.Clamp(stateManager.rigidBody2D.velocity.y, -stateManager.playerAttributes.wallSlidingSpeed, float.MaxValue));
            } else
            {
                stateManager.ChangeState(stateManager.jumpingState);
            }
        }
        
    }

    public override void ExitState(PlayerStateManager stateManager)
    {
        stateManager.animator.SetBool("Wall Sliding", false);
    }
}
