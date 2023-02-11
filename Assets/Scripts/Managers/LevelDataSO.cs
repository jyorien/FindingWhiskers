using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This ScriptableObject centralises data about the current level as it is being used by multple classes
/// </summary>
[CreateAssetMenu(fileName = "LevelDataSO", menuName = "ScriptableObjects/Level Data")]
public class LevelDataSO: ScriptableObject
{
    // keep track of whether clue is collected / Ghost is defeated to determine if player can complete the level when they touch the End Pole
    public bool isLevelCompleteRequirementMet;
    // keep track of whether the data in a level needs to be reset after player wins a level / loses all three lives and has to reset the entire game
    public bool isLevelNeedReset;
    // keep track of the current level that player is on so that other scenes that need to display the level can reference this
    public int currentLevel;
    // let other classes know if player completed the level
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
