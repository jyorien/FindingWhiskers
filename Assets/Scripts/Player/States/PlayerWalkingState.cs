using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This state happens when the player is trying to move back and forth horizontally while pressing the left or right buttons (A and D or Left and Right arrow keys)
/// Player can only transition to Standing, Jumping or Wall Sliding state from here.
/// </summary>
public class PlayerWalkingState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager stateManager)
    {
        stateManager.animator.SetBool("Walking", true);
    }

    public override void UpdateState(PlayerStateManager stateManager)
    {
        // if the player is not facing where they are supposed to, flip the GameObject horizontally
        if (stateManager.isFacingRight && stateManager.horizontalMovement < 0f ||
            !stateManager.isFacingRight && stateManager.horizontalMovement > 0f)
        {
            stateManager.FlipHorizontally();
        }

        // if the player lets go of the horizontal movement key, change to Standing state
        if (stateManager.horizontalMovement == 0)
        {
            stateManager.ChangeState(stateManager.standingState);
        }

        // if the player presses the jump button or walks off the floor, change to Jumping state
        if (stateManager.isJumpButtonPressed ||
            PlayerObstacleCollision.bottomColliderType == BottomColliderType.NONE)
        {
            stateManager.ChangeState(stateManager.jumpingState);
        }
    }

    public override void FixedUpdateState(PlayerStateManager stateManager)
    {
        // if player is touching a wall while not on the ground, slide on wall
        if (PlayerObstacleCollision.isTouchingWall &&
            PlayerObstacleCollision.bottomColliderType != BottomColliderType.FLOOR &&
            (stateManager.horizontalMovement > 0 && stateManager.isFacingRight ||
             stateManager.horizontalMovement < 0 && !stateManager.isFacingRight))
        {
            stateManager.ChangeState(stateManager.wallSlidingState);
        }


        switch (PlayerObstacleCollision.currentGroundType)
        {
            // on dirt ground, if player presses on a key to move horizontally, add velocity based on  the current velocity in the ScriptableObject
            case GroundType.DIRT:
                stateManager.rigidBody2D.velocity = new Vector2(stateManager.horizontalMovement * stateManager.playerAttributes.currentSpeed, stateManager.rigidBody2D.velocity.y);
                break;

            // on ice ground, if player presses on a key to move horizontally, accelerate the player until they reach maximum velocity defined by the ScriptableObject.
            // this is to simulate ice having "less friction" in rael life
            case GroundType.ICE:
                // constant acceleration until maximum velocity
                if (stateManager.playerAttributes.currentSpeedOnIce < stateManager.playerAttributes.maxExtraSpeedOnIce)
                {
                    stateManager.playerAttributes.currentSpeedOnIce += 0.2f;
                }
                stateManager.rigidBody2D.velocity = new Vector2(stateManager.horizontalMovement * stateManager.playerAttributes.currentSpeed, stateManager.rigidBody2D.velocity.y);

                // add the extra velocity
                stateManager.rigidBody2D.velocity += new Vector2(stateManager.horizontalMovement * stateManager.playerAttributes.currentSpeedOnIce, 0);
                break;
        }
    }

    public override void ExitState(PlayerStateManager stateManager)
    {
        stateManager.animator.SetBool("Walking", false);
    }
}
