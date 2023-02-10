using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Holds data about the player that needs to be constant as well as data that needs to be communicated between different classes
/// </summary>
[CreateAssetMenu(fileName = "PlayerAttributesDataSO", menuName = "ScriptableObjects/Player Attributes Data")]
public class PlayerAttributesDataSO : ScriptableObject
{
    // store attributes that will be consistent throughout the levels here
    public float gravityScale = 4f;
    public float fallGravityScale = 6f;
    public float maxExtraSpeedOnIce = 5f;
    public float currentSpeedOnIce = 0f;
    public float wallSlidingSpeed = 1f;
    public float wallJumpingTime = 0.4f;
    public float wallJumpingDuration = 0.4f;
    public Vector2 wallJumpingPower = new Vector2(9, 12);
    public UnityEvent<bool> OnFrozenStateChanged;
    public bool isFrozen { get; private set; }

    public float currentSpeed { get; private set; }
    public float currentJumpForce { get; private set; }

    private void OnEnable()
    {
        isFrozen = false;
    }

    public void ChangeAttributes(float newSpeed, float newJumpForce)
    {
        currentSpeed = newSpeed;
        currentJumpForce = newJumpForce;
    }

    public void ChangeFrozenState(bool newState)
    {
        isFrozen = newState;
        OnFrozenStateChanged.Invoke(newState);
    }
}
