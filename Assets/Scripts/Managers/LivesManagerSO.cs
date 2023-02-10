using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This ScriptableObject manages the lives in the game. There are two instances being used by the player and Ghost separately.
/// </summary>
[CreateAssetMenu(fileName = "LivesManagerSO", menuName = "ScriptableObjects/Lives Manager")]
public class LivesManagerSO : ScriptableObject
{
    public readonly int MaxLives = 3;
    public int Lives { get; private set; }
    // let other classes know if player lost a life or if lives are reset
    public UnityEvent<int> OnLivesChanged;

    private void OnEnable()
    {
        ResetLives();
    }

    public void ResetLives()
    {
        Lives = MaxLives;
        OnLivesChanged.Invoke(Lives);
    }
    public void DecreaseLife()
    {
        Lives -= 1;
        OnLivesChanged.Invoke(Lives);
    }
}
