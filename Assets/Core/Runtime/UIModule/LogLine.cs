using UnityEngine;
using UnityEngine.UI;

public class LogLine : MonoBehaviour
{
    [SerializeField]
    private Text textField;

    public void Initialize(string text)
    {
        textField.text = text;
    }
}
