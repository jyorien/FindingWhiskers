using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LevelCompleteUICanvas : MonoBehaviour
{
    [SerializeField] TMP_Text levelCompleteText;
    [SerializeField] TMP_Text timeTakenText;
    [SerializeField] TMP_Text personalBestTimeText;
    [SerializeField] Button btnContinue;

    private void Start()
    {
        int lastLevelPlayed = GameManager.Instance.GetLastLevelPlayed();
        int personalBest = Utils.GetLevelBestTiming(lastLevelPlayed);

        // format text and display on screen
        levelCompleteText.text = $"Level {lastLevelPlayed} Complete";
        timeTakenText.text = $"Time Taken:\n{GameManager.Instance.GetLastTimingRecorded()}";
        personalBestTimeText.text = $"Personal Best:\n{Utils.formatMillisecondsToDisplayTime(personalBest)}";
        btnContinue.onClick.AddListener(() =>
        {
            // proceed to next level
            GameManager.Instance.GoNextLevel();
        });
    }


}
