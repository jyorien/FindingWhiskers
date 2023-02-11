using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameOverUICanvas : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] LevelDataSO levelData;
    // Start is called before the first frame update
    private void Start()
    {
        restartButton.onClick.AddListener(() =>
        {
            // since this button is clicked when player loses all three lives, flag that the data in the level needs to be reset
            levelData.isLevelNeedReset = true;
            SceneManager.LoadScene("Level 1");
        });

        backToMenuButton.onClick.AddListener(() =>
        {
            // go back to menu
            SceneManager.LoadScene("Start Menu");
        });
    }
}
