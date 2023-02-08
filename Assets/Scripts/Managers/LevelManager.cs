using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// LevelManager manages the events emitted from the player's LivesManager and TimeManager ScriptableObjects.
/// It also handles the state in the level such as the current level number, and saving the player's personal best timing
/// </summary>
public class LevelManager : MonoBehaviour
{
    [SerializeField] private LivesManagerSO livesManager;
    [SerializeField] private TimeManagerSO timeManager;
    [SerializeField] private LevelDataSO levelData;

    private void Awake()
    {
        levelData.isLevelCompleteRequirementMet = false;

        if (levelData.isLevelNeedReset)
        {
            levelData.isLevelNeedReset = false;
            livesManager.ResetLives();
            timeManager.ResetTimer();
        }

        livesManager.OnLivesChanged.AddListener(HandleLivesChanged);
        levelData.OnWin.AddListener(OnWin);

        timeManager.StartTimer();
        levelData.currentLevel = SceneManager.GetActiveScene().buildIndex;

    }

    private void OnDestroy()
    {
        livesManager.OnLivesChanged.RemoveListener(HandleLivesChanged);
        levelData.OnWin.RemoveListener(OnWin);
    }

    private void HandleLivesChanged(int livesLeft)
    {
        if (livesLeft < 1)
        {
            // if player loses all 3 lives, end the game
            timeManager.StopTimer();
            SceneManager.LoadScene("Game Over", LoadSceneMode.Additive);
        }
        else
        {
            // if player loses a life, restart level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    private void OnWin()
    {
        SavePersonalBestTiming();
        SceneManager.LoadScene("Level Complete", LoadSceneMode.Additive);

    }

    private void SavePersonalBestTiming()
    {
        timeManager.StopTimer();
        long elapsedTimeInMilliseconds = timeManager.GetTiming();

        // check if user beat their personal best or does not have one yet, if so store new personal best
        if (elapsedTimeInMilliseconds < Utils.GetLevelBestTiming(levelData.currentLevel) ||
            Utils.GetLevelBestTiming(levelData.currentLevel) == 0)
        {
            Utils.SaveLevelBestTiming(levelData.currentLevel, (int) elapsedTimeInMilliseconds);
        }
    }
}
