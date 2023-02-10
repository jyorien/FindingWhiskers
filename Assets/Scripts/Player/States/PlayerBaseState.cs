using UnityEngine;

/// <summary>
/// This is the base state that the other player states must inherit from
/// </summary>
public abstract class PlayerBaseState
{
    public abstract void EnterState(PlayerStateManager stateManager);

    public abstract void UpdateState(PlayerStateManager stateManager);

    public abstract void FixedUpdateState(PlayerStateManager stateManager);

    public abstract void ExitState(PlayerStateManager stateManager);
}
