using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameUICanvas : MonoBehaviour
{
    [SerializeField] TMP_Text stopwatchText;

    // Update is called once per frame
    void Update()
    {
        // get stopwatch timing and format every frame
        stopwatchText.text = Utils.formatMillisecondsToDisplayTime(GameManager.Instance.GetGameTimeElapsedInMiliseconds());
    }
}
