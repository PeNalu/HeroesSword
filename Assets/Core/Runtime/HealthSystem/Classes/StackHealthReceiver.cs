using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class StackHealthReceiver : MonoBehaviour
{
    //Stored required components.
    private StackHealth stackHealth;
    private TextMesh textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMesh>();
        stackHealth = GetComponentInParent<StackHealth>();
    }

    private void Start()
    {
        stackHealth.OnStackDeath += Print;
        Print(stackHealth.GetCurrentStack());
    }

    private void Print(int count)
    {
        textMesh.text = $"{stackHealth.GetCurrentStack()}"; 
    }
}
