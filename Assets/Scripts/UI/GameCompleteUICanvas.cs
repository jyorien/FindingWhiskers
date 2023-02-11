using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameCompleteUICanvas : MonoBehaviour
{
    [SerializeField] private Button btnBackToMenu;
    // Start is called before the first frame update
    private void Start()
    {
        // when player completes the entire game, the only button they can press is the Back button to back to the Start Menu
        btnBackToMenu.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Start Menu");
        });
    }
}
