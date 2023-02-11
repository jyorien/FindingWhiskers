using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameUICanvas : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text stopwatchText;
    [SerializeField] private TMP_Text requirementText;
    [SerializeField] private GameObject[] lives;

    [Header("Scriptable Objects")]
    [SerializeField] private LivesManagerSO livesManager;
    [SerializeField] private TimeManagerSO timeManager;
    [SerializeField] private LevelDataSO levelData;

    private void Awake()
    {
        // subscribe so that when player loses and is shown the Game Over screen in the same scene, the number of hearts will be updated to zero
        livesManager.OnLivesChanged.AddListener(DisplayLivesLeft);
    }

    private void Start()
    {
        // the lives do not update when the player starts a level since updating the lives is event-driven,
        // hence this forces the number lives to display when level starts
        DisplayLivesLeft(livesManager.Lives);
    }

    private void OnDestroy()
    {
        livesManager.OnLivesChanged.RemoveListener(DisplayLivesLeft);
    }

    // Update is called once per frame
    private void Update()
    {
        // get stopwatch timing and format every frame
        stopwatchText.text = Utils.FormatMillisecondsToDisplayTime(timeManager.GetTiming());
        if (levelData.isLevelCompleteRequirementMet)
        {
            requirementText.text = "1 / 1";
        }
        else
        {
            requirementText.text = "0 / 1";
        }
    }

    /// <summary>
    /// Enables or disables the hearts based on number of lives left.
    /// </summary>
    /// <param name="livesLeft">Number of lives to display</param>
    private void DisplayLivesLeft(int livesLeft)
    {
        // display the heart if current index is less than the number of lives to display
        // otherwise hide the heart
        for (int i = 0; i < livesManager.MaxLives; i++)
        {
            lives[i].SetActive(i < livesLeft);
        }
    }
}
