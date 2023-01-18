using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                Debug.LogError("GameManager is null");
            }
            // persist GameManager throughout the scenes
            DontDestroyOnLoad(_instance);
            return _instance;
        }
    }

    private int sceneCount;
    private bool isClueFound = false;

    private void Awake()
    {
        sceneCount = SceneManager.sceneCountInBuildSettings;
        _instance = this;
    }

    public void GoNextScene()
    {
        // go to next scene based on index
        int buildIndex = SceneManager.GetActiveScene().buildIndex;

        if (buildIndex < sceneCount)
        {
            SceneManager.LoadScene(buildIndex + 1);
        }
    }

    public void OnClueFound()
    {
        isClueFound = true;
    }


    public void OnFinishPoleTouched()
    {
        // determine if user completed the stage yet
        // based on whether he has collected the clue
        if (!isClueFound)
            return;

        OnGameWin();
    }

    public void OnGameWin()
    {
        /* if player wins, go to next level and reset the "clue found" state
        for the next level */
        isClueFound = false;
        GoNextScene();

    }

    public void OnGameLose()
    {
        // if player loses, restart level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
