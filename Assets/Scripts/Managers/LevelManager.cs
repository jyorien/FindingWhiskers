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
        // always reset this state to false when a level is loaded
        // because the player might have died after collecting a clue and the ScriptableObject persists state even when scene reloads
        levelData.isLevelCompleteRequirementMet = false;

        // if the player loses all three lives or completes a level, the data in the ScriptableObjects need to be reset for the new level being loaded
        if (levelData.isLevelNeedReset)
        {
            levelData.isLevelNeedReset = false;
            livesManager.ResetLives();
            timeManager.ResetTimer();
        }

        // subscribe to the events from the ScriptableObjects
        livesManager.OnLivesChanged.AddListener(HandleLivesChanged);
        levelData.OnWin.AddListener(OnWin);

        // start the timer when level loads
        // StartTimer() will handle whether to start the timer or do nothing if there is a timer already running
        timeManager.StartTimer();
        // the level number corresponds to the scene's build index
        levelData.currentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    private void OnDestroy()
    {
        // stop subscribing to the events
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

    /// <summary>
    /// Gets called when player touches the End Pole with the Level Complete Requirement met.
    /// It updates the level's best timing if needed, and adds the Level Complete scene to the currently loaded scene.
    /// </summary>
    private void OnWin()
    {
        timeManager.StopTimer();
        SavePersonalBestTiming();
        SceneManager.LoadScene("Level Complete", LoadSceneMode.Additive);

    }

    /// <summary>
    /// Retrieves the level's best timing to compare with the player's current timing for the level.
    /// If the player completed the level in less time, it updates the player's best timing in PlayerPrefs.
    /// </summary>
    private void SavePersonalBestTiming()
    {   
        long elapsedTimeInMilliseconds = timeManager.GetTiming();

        // store new personal best if user beat their personal best or does not have one yet 
        if (elapsedTimeInMilliseconds < Utils.GetLevelBestTiming(levelData.currentLevel) ||
            Utils.GetLevelBestTiming(levelData.currentLevel) == 0)
        {
            Utils.SaveLevelBestTiming(levelData.currentLevel, (int) elapsedTimeInMilliseconds);
        }
    }
}
