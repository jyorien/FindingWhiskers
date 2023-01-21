using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameUICanvas : MonoBehaviour
{
    [SerializeField] TMP_Text stopwatchText;
    [SerializeField] GameObject[] lives;
    private void Start()
    {
        /* since the scene reloads when the player loses, the lives only need to be
         * rendered once in Start()
         */
        int livesLeft = GameManager.Instance.GetLivesLeft();
  
        int livesUsed = 3 - livesLeft;
        for (int i = 0; i < livesUsed; i++)
        {

            lives[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // get stopwatch timing and format every frame
        stopwatchText.text = Utils.formatMillisecondsToDisplayTime(GameManager.Instance.GetGameTimeElapsedInMiliseconds());
    }
}
