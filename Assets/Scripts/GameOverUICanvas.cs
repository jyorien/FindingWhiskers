using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameOverUICanvas : MonoBehaviour
{
    [SerializeField] Button restartButton;
    [SerializeField] Button backToMenuButton;
    // Start is called before the first frame update
    void Start()
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
