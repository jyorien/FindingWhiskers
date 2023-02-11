using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostLivesUI : MonoBehaviour
{
    [SerializeField] private GameObject[] ghostLives;
    [SerializeField] private LivesManagerSO ghostLivesManager;

    private void Awake()
    {
        ghostLivesManager.OnLivesChanged.AddListener(DisplayGhostLivesLeft);
    }

    private void OnDestroy()
    {
        ghostLivesManager.OnLivesChanged.RemoveListener(DisplayGhostLivesLeft);
    }

    /// <summary>
    /// Enables or disables the hearts based on number of lives left.
    /// </summary>
    /// <param name="livesLeft">Number of lives to display</param>
    private void DisplayGhostLivesLeft(int livesLeft)
    {
        for (int i = 0; i < ghostLivesManager.MaxLives; i++)
        {
            // display the heart if current index is less than the number of lives to display
            // otherwise hide the heart
            ghostLives[i].SetActive(i < livesLeft);
        }
    }
}
