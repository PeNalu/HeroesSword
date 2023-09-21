using UnityEngine;

public class CellSelector : MonoBehaviour
{
    [SerializeField]
    private float zOffset = 0.2f;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public void Show(Vector3 position, Color color)
    {
        position.z = zOffset;
        transform.position = position;
        spriteRenderer.color = color;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
