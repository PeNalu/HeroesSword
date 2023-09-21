using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MoveButtonSwitcher : MonoBehaviour
{
    //Stored required components.
    private GameManager gameManager;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        gameManager = GameManager.GetRuntimeInstance();
    }

    private void Update()
    {
        if(gameManager.GetActiveUnit() != null && gameManager.GetActiveUnit().GetMovementPoint() != 0)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
}
