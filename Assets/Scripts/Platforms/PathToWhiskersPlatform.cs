using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script handles activating the path to whiskers when player beats Ghost in the last level
/// </summary>
public class PathToWhiskersPlatform : MonoBehaviour
{
    [SerializeField] LivesManagerSO ghostLivesManager;
    [SerializeField] GameObject movingPlatform;
    private void Awake()
    {
        ghostLivesManager.OnLivesChanged.AddListener(HandleLivesChanged);
    }

    private void OnDestroy()
    {
        ghostLivesManager.OnLivesChanged.AddListener(HandleLivesChanged);
    }

    private void HandleLivesChanged(int livesLeft)
    {
        if (livesLeft == 0)
        {
            movingPlatform.SetActive(true);
        }
    }
}
