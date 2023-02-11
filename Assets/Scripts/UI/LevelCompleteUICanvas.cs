using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class LevelCompleteUICanvas : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private TimeManagerSO timeManager;
    [SerializeField] private LevelDataSO levelData;

    [Header("Components")]
    [SerializeField] private TMP_Text levelCompleteText;
    [SerializeField] private TMP_Text timeTakenText;
    [SerializeField] private TMP_Text personalBestTimeText;
    [SerializeField] private Button btnContinue;

    private void Start()
    {
        // get data to display and format the data
        int lastLevelPlayed = levelData.currentLevel;
        int personalBest = Utils.GetLevelBestTiming(lastLevelPlayed);
        string personalBestTiming = Utils.FormatMillisecondsToDisplayTime(personalBest);
        string levelTiming = Utils.FormatMillisecondsToDisplayTime(timeManager.GetTiming());

        // format text and display on screen
        levelCompleteText.text = $"Level {lastLevelPlayed} Complete";
        timeTakenText.text = $"Time Taken:\n{levelTiming}";
        personalBestTimeText.text = $"Personal Best:\n{personalBestTiming}";
        btnContinue.onClick.AddListener(() =>
        {
            // proceed to next level
            SceneManager.LoadScene(lastLevelPlayed + 1);
        });
    }
}
