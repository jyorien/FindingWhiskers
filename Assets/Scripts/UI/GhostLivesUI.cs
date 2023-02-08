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

    private void DisplayGhostLivesLeft(int livesLeft)
    {
        // enable or disable the hearts based on number of lives left
        for (int i = 0; i < ghostLivesManager.MaxLives; i++)
        {
            ghostLives[i].SetActive(i < livesLeft);
        }
    }
}
