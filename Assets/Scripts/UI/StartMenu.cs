using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StartMenu : MonoBehaviour
{
    [SerializeField] Button startButton;
    // Start is called before the first frame update
    void Start()
    {
        // start the game
        startButton.onClick.AddListener(() => {
            GameManager.Instance.GoToLevel(1);
        });
    }
}
