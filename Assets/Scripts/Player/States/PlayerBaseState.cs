using UnityEngine;

/// <summary>
/// This is the base state that the other player states must inherit from.
/// </summary>
public abstract class PlayerBaseState
{
    /// <summary>
    /// This gets called whenever the Finite State Machine enters into the state.
    /// </summary>
    public abstract void EnterState(PlayerStateManager stateManager);

    /// <summary>
    /// This gets called in the Finite State Machine's Update method when it set to the current state.
    /// </summary>
    public abstract void UpdateState(PlayerStateManager stateManager);

    /// <summary>
    /// This gets called in the Finite State Machine's FixedUpdate method when it set to the current state.
    /// </summary>
    public abstract void FixedUpdateState(PlayerStateManager stateManager);

    /// <summary>
    /// This gets called whenever the Finite Sate Machine exits the state to transition into a new state.
    /// </summary>
    public abstract void ExitState(PlayerStateManager stateManager);
}
