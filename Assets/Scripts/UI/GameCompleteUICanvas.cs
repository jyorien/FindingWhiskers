using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameCompleteUICanvas : MonoBehaviour
{
    [SerializeField] private Button btnBackToMenu;
    // Start is called before the first frame update
    private void Start()
    {
        btnBackToMenu.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Start Menu");
        });
    }
}
