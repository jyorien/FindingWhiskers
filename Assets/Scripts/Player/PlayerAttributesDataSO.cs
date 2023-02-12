using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Holds data about the player that needs to be constant as well as data about the player that need to be communicated between different classes.
/// </summary>
[CreateAssetMenu(fileName = "PlayerAttributesDataSO", menuName = "ScriptableObjects/Player Attributes Data")]
public class PlayerAttributesDataSO : ScriptableObject
{
    // store attributes that will be constant throughout the levels here
    public float gravityScale = 4f;
    public float fallGravityScale = 6f;
    public float maxExtraSpeedOnIce = 5f;
    public float currentSpeedOnIce = 0f;
    public float wallSlidingSpeed = 1f;
    public float wallJumpingTime = 0.4f;
    public float wallJumpingDuration = 0.4f;
    public Vector2 wallJumpingPower = new Vector2(9, 12);

    // store data that need to be communicated between different classes
    public float currentSpeed { get; private set; }
    public float currentJumpForce { get; private set; }
    public bool isFrozen { get; private set; }
    public UnityEvent<bool> OnFrozenStateChanged;

    private void OnEnable()
    {
        // always reset frozen state when game starts
        isFrozen = false;
    }

    /// <summary>
    /// Handles updating the current speed and jump force for other classes to access.
    /// </summary>
    /// <param name="newSpeed">The new speed to set the current speed to.</param>
    /// <param name="newJumpForce">The new jump force to set the current jump force to.</param>
    public void ChangeAttributes(float newSpeed, float newJumpForce)
    {
        currentSpeed = newSpeed;
        currentJumpForce = newJumpForce;
    }

    /// <summary>
    /// Allows the frozen state to be updated from different scripts and notifies listeners.
    /// </summary>
    /// <param name="newState">The new frozen state to be set.</param>
    public void ChangeFrozenState(bool newState)
    {
        isFrozen = newState;
        OnFrozenStateChanged.Invoke(newState);
    }
}
