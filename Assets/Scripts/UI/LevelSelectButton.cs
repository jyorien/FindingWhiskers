using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelSelectButton : MonoBehaviour
{
    // set the level to load from inspector to reuse the script in multiple buttons
    [SerializeField] private int level;
    private Button btn; 
    // Start is called before the first frame update
    private void Start()
    {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            // load the level based on value in inspector
            SceneManager.LoadScene($"Level {level}");
        });
    }
}
