using UnityEngine;

/// <summary>
/// This state handles adding force when the player presses the jump button, or when the player is in air.
/// If player is in air without pressing the jump button, this state handles changing to the Jumping animation
/// and allows the player to move horizontally in air.
/// Player can only transition to Standing, Walking or Wall Sliding state from here.
/// </summary>
public class PlayerJumpingState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager stateManager)
    {
        stateManager.animator.SetBool("Jumping", true);
        // only add force when player presses the button
        if (stateManager.isJumpButtonPressed)
        {
            stateManager.rigidBody2D.AddForce(Vector2.up * stateManager.playerAttributes.currentJumpForce, ForceMode2D.Impulse);
        }
    }

    public override void UpdateState(PlayerStateManager stateManager)
    {

        // if the player is not facing where they are supposed to, flip the GameObject horizontally
        if (stateManager.isFacingRight && stateManager.horizontalMovement < 0f ||
            !stateManager.isFacingRight && stateManager.horizontalMovement > 0f)
        {
            stateManager.FlipHorizontally();
        }
    }

    public override void FixedUpdateState(PlayerStateManager stateManager)
    {
        // to achieve faster falling, change gravity to a higher value when falling down
        stateManager.rigidBody2D.gravityScale = stateManager.rigidBody2D.velocity.y > 0 ? stateManager.playerAttributes.gravityScale : stateManager.playerAttributes.fallGravityScale;

        // if player is touching a wall while not on the ground, change to Wall Sliding state
        if (PlayerObstacleCollision.isTouchingWall &&
            PlayerObstacleCollision.bottomColliderType != BottomColliderType.FLOOR &&
            (stateManager.horizontalMovement > 0 && stateManager.isFacingRight ||
             stateManager.horizontalMovement < 0 && !stateManager.isFacingRight))
        {
            stateManager.ChangeState(stateManager.wallSlidingState);
        }

        // if player touches ground when falling downwards, reset gravityScale and change state
        if (
            PlayerObstacleCollision.bottomColliderType != BottomColliderType.NONE)
        {
            stateManager.ChangeState(stateManager.horizontalMovement == 0 ? stateManager.standingState : stateManager.walkingState);
        }

        // allow the player to move horizontally in air
        if (stateManager.horizontalMovement != 0)
        {
            switch (PlayerObstacleCollision.lastTouchedGroundType)
            {
                // if the player was on Dirt before jumping, the current speed will be used to move the player.
                case GroundType.DIRT:
                    stateManager.rigidBody2D.velocity = new Vector2(stateManager.horizontalMovement * stateManager.playerAttributes.currentSpeed, stateManager.rigidBody2D.velocity.y);
                    break;
                // if the player was on ice before jumping, the current speed + extra speed added from accelerating on ice will be used to move the player.
                case GroundType.ICE:
                    stateManager.rigidBody2D.velocity = new Vector2(stateManager.horizontalMovement * stateManager.playerAttributes.currentSpeed, stateManager.rigidBody2D.velocity.y);
                    stateManager.rigidBody2D.velocity += new Vector2(stateManager.horizontalMovement * stateManager.playerAttributes.currentSpeedOnIce, 0);
                    break;
            }
        }
    }

    public override void ExitState(PlayerStateManager stateManager)
    {
        stateManager.rigidBody2D.gravityScale = stateManager.playerAttributes.gravityScale;
        stateManager.animator.SetBool("Jumping", false);
    }
}
