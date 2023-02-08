using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StartMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    // Start is called before the first frame update
    private void Start()
    {
        // start the game
        startButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Level 1");
        });
    }
}
