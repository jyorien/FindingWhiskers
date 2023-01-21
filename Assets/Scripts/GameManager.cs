using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    // make GameManager a singleton so that only one exists throughout the game
    // we can reference GameManager from other classes without instantiating it
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                UnityEngine.Debug.LogError("GameManager is null");
            }
            // persist GameManager throughout the scenes
            DontDestroyOnLoad(_instance);
            return _instance;
        }
    }

    private bool isClueFound = false;
    // create timer to record time taken to complete a level
    // timer will be managed in the GameManager to easily manage the data throughout the scenes
    private Stopwatch timer;
    // track the current level
    private int currentLevelBuildIndex = 0;
    // store time taken to complete a level here (formatted)
    private string lastTimingSaved;

    // store player's lives that will reset every level
    private int livesLeft = 3;

    private void Awake()
    {
        _instance = this;
        timer = new Stopwatch();
    }

    public void GoNextLevel()
    {
        // reset timer and lives for next level
        timer.Reset();
        livesLeft = 3;
        // store index of next scene
        currentLevelBuildIndex += 1;

        // TODO: Levels 1 - 3 only, need to add condition for boss level
        if (currentLevelBuildIndex > 0 && currentLevelBuildIndex < 4)
        {
            SceneManager.LoadScene(currentLevelBuildIndex);

            // start tracking the time for the player to complete the level
            timer.Start();
        }
    }

    public void OnClueFound()
    {
        isClueFound = true;
    }


    public void OnFinishPoleTouched()
    {
        UnityEngine.Debug.Log($"Lives Left: {livesLeft}");

        // determine if user completed the stage yet
        // based on whether he has collected the clue
        // TODO: need to add condition for boss level
        if (!isClueFound)
            return;

        OnGameWin();
    }

    public void OnGameWin()
    {
        /* if player wins, go to next level and reset states
        for the next level */
        isClueFound = false;

        // stop timer every level
        timer.Stop();
        // format into 00:00:000 to display in Level Complete Scene
        lastTimingSaved = Utils.formatMillisecondsToDisplayTime(timer.ElapsedMilliseconds);
        // check if user beat their personal best, if so store new personal best
        if (timer.ElapsedMilliseconds < Utils.GetLevelBestTiming(currentLevelBuildIndex) || Utils.GetLevelBestTiming(currentLevelBuildIndex) == 0)
        {
            Utils.SaveLevelBestTiming(currentLevelBuildIndex, (int)timer.ElapsedMilliseconds);
        }

        SceneManager.LoadScene("Level Complete", LoadSceneMode.Additive);
    }

    public void OnGameLose()
    {
        // if player loses, restart level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        livesLeft -= 1;
    }

    public long GetGameTimeElapsedInMiliseconds()
    {
        return timer.ElapsedMilliseconds;
    }

    public string GetLastTimingRecorded()
    {
        return lastTimingSaved;
    }

    public int GetLastLevelPlayed()
    {
        return currentLevelBuildIndex;
    }

    public int GetLivesLeft()
    {
        return livesLeft;
    }
}
