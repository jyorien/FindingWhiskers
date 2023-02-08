using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "ScriptableObjects/Level Data")]
public class LevelDataSO: ScriptableObject
{
    // keep track of whether clue is collected / Ghost is defeated to determine if player can complete the level when they touch the End Pole
    public bool isLevelCompleteRequirementMet;
    // keep track if a level needs to be reset after player wins / loses all three lives and has to reset the entire game
    public bool isLevelNeedReset;
    // keep track of current level that player is on so that other scenes that need to display the level can reference this
    public int currentLevel;
    // let other classes know if player won the game
    public UnityEvent OnWin;

    private void OnEnable()
    {
        isLevelCompleteRequirementMet = false;
    }

    public void TriggerOnWin()
    {
        // only complete level if player collected clue / defeated boss
        if (isLevelCompleteRequirementMet)
        {
            isLevelNeedReset = true;
            OnWin.Invoke();
        }
    }

}
