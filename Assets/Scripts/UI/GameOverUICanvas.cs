using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameOverUICanvas : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button backToMenuButton;
    // Start is called before the first frame update
    private void Start()
    {
        restartButton.onClick.AddListener(() =>
        {
            // let GameManager handle the reset to reset the game state and scene
            GameManager.Instance.ResetGameFromLevelOne();
        });

        backToMenuButton.onClick.AddListener(() =>
        {
            // go back to menu
            SceneManager.LoadScene("Start Menu");
        });
    }
}
