using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LevelCompleteUICanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text levelCompleteText;
    [SerializeField] private TMP_Text timeTakenText;
    [SerializeField] private TMP_Text personalBestTimeText;
    [SerializeField] private Button btnContinue;

    private void Start()
    {
        int lastLevelPlayed = GameManager.Instance.currentLevelBuildIndex;
        int personalBest = Utils.GetLevelBestTiming(lastLevelPlayed);

        // format text and display on screen
        levelCompleteText.text = $"Level {lastLevelPlayed} Complete";
        timeTakenText.text = $"Time Taken:\n{GameManager.Instance.lastTimingSaved}";
        personalBestTimeText.text = $"Personal Best:\n{Utils.formatMillisecondsToDisplayTime(personalBest)}";
        btnContinue.onClick.AddListener(() =>
        {
            // proceed to next level
            GameManager.Instance.GoNextLevel();
        });
    }


}
