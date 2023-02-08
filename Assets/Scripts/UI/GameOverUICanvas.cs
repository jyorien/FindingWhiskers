using System.Collections;
using System.Collections.Generic;
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
