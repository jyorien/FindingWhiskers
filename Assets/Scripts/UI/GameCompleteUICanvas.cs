using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameCompleteUICanvas : MonoBehaviour
{
    [SerializeField] Button btnBackToMenu;
    // Start is called before the first frame update
    void Start()
    {
        btnBackToMenu.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Start Menu");
        });
    }
}
