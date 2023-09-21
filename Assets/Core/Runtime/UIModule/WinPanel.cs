using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    [SerializeField]
    private Text winTextField;

    public void Show(string text)
    {
        winTextField.text = text;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        winTextField.text = "";
        gameObject.SetActive(false);
    }

    public void Restart()
    {
        Hide();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
