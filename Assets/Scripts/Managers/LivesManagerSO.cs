using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This ScriptableObject manages the lives in the game. There are two instances being used by the player and Ghost separately.
/// </summary>
[CreateAssetMenu(fileName = "LivesManagerSO", menuName = "ScriptableObjects/Lives Manager")]
public class LivesManagerSO : ScriptableObject
{
    public readonly int maxLives = 3;
    public int lives { get; private set; }
    // let other classes know if a life is lost or if lives are reset
    public UnityEvent<int> OnLivesChanged;

    private void OnEnable()
    {
        ResetLives();
    }

    /// <summary>
    /// Resets the current number of lives to the max number of lives and notifies listeners of the change
    /// </summary>
    public void ResetLives()
    {
        lives = maxLives;
        OnLivesChanged.Invoke(lives);
    }

    /// <summary>
    /// Decreases the current number of lives and notifies listeners of the change
    /// </summary>
    public void DecreaseLife()
    {
        lives -= 1;
        OnLivesChanged.Invoke(lives);
    }
}
