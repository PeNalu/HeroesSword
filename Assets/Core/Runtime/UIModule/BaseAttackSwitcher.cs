using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BaseAttackSwitcher : MonoBehaviour
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
        if (gameManager.GetActiveUnit() != null && gameManager.GetActiveUnit().CanAttack())
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
}
