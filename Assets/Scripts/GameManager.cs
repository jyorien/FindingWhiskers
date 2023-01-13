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
            DontDestroyOnLoad(_instance);
            return _instance;
        }
    }

    int sceneCount;

    private void Awake()
    {
        sceneCount = SceneManager.sceneCount;
        _instance = this;
    }

    // if player wins, go to next level
    public void OnGameWin()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        if (buildIndex < sceneCount + 1)
        {
            SceneManager.LoadScene(buildIndex + 1);
        }
    }

    // if player loses, restart level
    public void OnGameLose()
    {
        Debug.Log("L");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
