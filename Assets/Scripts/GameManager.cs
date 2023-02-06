using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    /* make GameManager a singleton so that only one exists throughout the game
     * we can reference GameManager from other classes without instantiating it
     */
    //private static GameManager _instance;
    public static GameManager Instance;

    // store condition to complete level
    public bool isLevelCompleteRequirementMet;

    /* create timer to record time taken to complete a level
     * timer will be managed in the GameManager to easily manage the data throughout the scenes
     */
    private Stopwatch timer;

    // track the current level
    public int currentLevelBuildIndex { get; private set; }

    /* store time taken to complete a level here (formatted)
     * only allow variable's getter to be called outside of this class
     */
    public string lastTimingSaved { get; private set; }

    // store player's lives that will reset every level
    public int livesLeft { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        } else
        {
            Destroy(gameObject);
        }
        // persist GameManager throughout the scenes
        DontDestroyOnLoad(Instance);
    }

    private void Start()
    {
        // initalise variables
        timer = new Stopwatch();
        ResetLevelStates();
        currentLevelBuildIndex = 0;
    }

    private void ResetLevelStates()
    {
        isLevelCompleteRequirementMet = false;
        timer.Reset();
        livesLeft = 3;
    }

    public void GoNextLevel()
    {
        // if player wins, go to next level based on the build index and reset states for the next level

        ResetLevelStates();

        // store index of next scene
        currentLevelBuildIndex += 1;

        if (currentLevelBuildIndex > 0 && currentLevelBuildIndex < 6)
        {
            SceneManager.LoadScene(currentLevelBuildIndex);

            // start tracking the time for the player to complete the level
            timer.Start();
        }
    }

    public void GoToLevel(int level)
    {
        // go straight to a specific level

        currentLevelBuildIndex = level;
        ResetLevelStates();
        SceneManager.LoadScene(level);
        timer.Start();
    }

    public void ResetGameFromLevelOne()
    {
        // reset timer and lives
        ResetLevelStates();

        // reset the index to load and increment from level one
        currentLevelBuildIndex = 1;

        if (currentLevelBuildIndex > 0 && currentLevelBuildIndex < 4)
        {
            SceneManager.LoadScene(currentLevelBuildIndex);
            // start tracking the time for the player to complete the level
            timer.Start();
        }
    }

    public void OnGameWin()
    {
        // stop timer every level
        timer.Stop();
        // format into 00:00:000 to display in Level Complete Scene
        lastTimingSaved = Utils.formatMillisecondsToDisplayTime(timer.ElapsedMilliseconds);

        // check if user beat their personal best or does not have one yet, if so store new personal best
        if (timer.ElapsedMilliseconds < Utils.GetLevelBestTiming(currentLevelBuildIndex) ||
            Utils.GetLevelBestTiming(currentLevelBuildIndex) == 0)
        {
            Utils.SaveLevelBestTiming(currentLevelBuildIndex, (int)timer.ElapsedMilliseconds);
        }

        SceneManager.LoadScene("Level Complete", LoadSceneMode.Additive);
    }

    public void OnGameLose()
    {
        livesLeft -= 1;
        isLevelCompleteRequirementMet = false;

        // get the GameUICanvas of the current level to make the last life disappear
        GameUICanvas gameUICanvas = GameObject.Find("GameCanvas").GetComponent<GameUICanvas>();
        gameUICanvas.DisplayLivesLeft(livesLeft);

        /* timer will continue even if player loses lives.
         * it will only stop when the player loses the entire game
         */
        if (livesLeft < 1)
        {
            // if player loses all 3 lives, end the game
            timer.Stop();
            SceneManager.LoadScene("Game Over", LoadSceneMode.Additive);
        } else
        {
            // if player loses a life, restart level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // only expose ElapsedMilliseconds from timer
    public long GetGameTimeElapsedInMilliseconds()
    {
        return timer.ElapsedMilliseconds;
    }
}
