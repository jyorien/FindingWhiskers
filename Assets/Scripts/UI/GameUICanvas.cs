using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameUICanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text stopwatchText;
    [SerializeField] private TMP_Text requirementText;
    [SerializeField] private GameObject[] lives;
    [SerializeField] private LivesManagerSO livesManager;
    [SerializeField] private TimeManagerSO timeManager;
    [SerializeField] private LevelDataSO levelData;

    private void Awake()
    {
        livesManager.OnLivesChanged.AddListener(DisplayLivesLeft);
    }

    private void Start()
    {
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
        stopwatchText.text = Utils.formatMillisecondsToDisplayTime(timeManager.GetTiming());
        if (levelData.isLevelCompleteRequirementMet)
        {
            requirementText.text = "1 / 1";
        }
        else
        {
            requirementText.text = "0 / 1";
        }
    }

    private void DisplayLivesLeft(int livesLeft)
    {
        // enable or disable the hearts based on number of lives left
        for (int i = 0; i < livesManager.MaxLives; i++)
        {
            lives[i].SetActive(i < livesLeft);
        }
    }
}
