using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsPanel : MonoBehaviour
{
    [SerializeField]
    private Text nameField;

    [SerializeField]
    private Text damageField;

    [SerializeField]
    private Text movePointField;

    [SerializeField]
    private Text descriptionField;

    //Stored required components.
    private UnitController controller;

    private void Update()
    {
        if(controller != null)
        {
            UpdateView();
        }
    }

    private void UpdateView()
    {
        string color = controller.GetTeam() == GameManager.Team.Blue ? "blue" : "red";
        nameField.text = $"<color={color}>{controller.gameObject.name}</color>";
        damageField.text = $"Damage: <color=red>{controller.GetMinDamage()} - {controller.GetMaxDamage()}</color>";
        movePointField.text = $"Move Point: <color=green>{controller.GetMovementPoint()}</color>";
        descriptionField.text = $"{controller.GetDescription()}";
    }

    public void SetCharacter(UnitController unitController)
    {
        controller = unitController;
    }
}
