using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameUICanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text stopwatchText;
    [SerializeField] private TMP_Text requirementText;
    [SerializeField] private GameObject[] lives;
    private void Start()
    {
        // render number of lives left for the level when scene starts
        int livesLeft = GameManager.Instance.livesLeft;
        DisplayLivesLeft(livesLeft);
    }

    // Update is called once per frame
    private void Update()
    {
        // get stopwatch timing and format every frame
        stopwatchText.text = Utils.formatMillisecondsToDisplayTime(GameManager.Instance.GetGameTimeElapsedInMilliseconds());
        if (GameManager.Instance.isLevelCompleteRequirementMet)
        {
            requirementText.text = "1 / 1";
        } else
        {
            requirementText.text = "0 / 1";
        }
    }

    public void DisplayLivesLeft(int livesLeft)
    {
        // get how many hearts to hide
        int livesUsed = 3 - livesLeft;

        // in case lives go over 3, set it to 3 to avoid making index out of bounds
        if (livesUsed > 3) livesUsed = 3;

        // disable the hearts based on index
        for (int i = 0; i < livesUsed; i++)
        {
            lives[i].SetActive(false);
        }
    }
}
