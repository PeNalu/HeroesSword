using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UnitStateSlot : MonoBehaviour
{
    [SerializeField]
    private Text nameField;

    //Stored required components.
    private Button button;

    //Stored required properties.
    private string stateName;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void Initialize(string stateName)
    {
        this.stateName = stateName;
        nameField.text = stateName;
    }

    private void OnClick()
    {
        OnClickCallback?.Invoke(stateName);
    }

    #region [Event Callback Functions]
    public event Action<string> OnClickCallback;
    #endregion
}
