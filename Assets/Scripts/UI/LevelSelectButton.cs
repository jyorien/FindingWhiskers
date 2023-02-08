using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private int level;
    Button btn; 
    // Start is called before the first frame update
    private void Start()
    {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene($"Level {level}");
        });
    }
}
