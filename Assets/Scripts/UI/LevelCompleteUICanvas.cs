using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class LevelCompleteUICanvas : MonoBehaviour
{
    [SerializeField] TimeManagerSO timeManager;
    [SerializeField] LevelDataSO levelData;
    [SerializeField] private TMP_Text levelCompleteText;
    [SerializeField] private TMP_Text timeTakenText;
    [SerializeField] private TMP_Text personalBestTimeText;
    [SerializeField] private Button btnContinue;

    private void Start()
    {
        int lastLevelPlayed = levelData.currentLevel;
        int personalBest = Utils.GetLevelBestTiming(lastLevelPlayed);
        string personalBestTiming = Utils.formatMillisecondsToDisplayTime(personalBest);
        string levelTiming = Utils.formatMillisecondsToDisplayTime(timeManager.GetTiming());

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
