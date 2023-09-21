using System.Collections.Generic;
using UnityEngine;

public class SpriteOrderer : MonoBehaviour
{
    [SerializeField]
    private List<UnitController> unitControllers;

    //Stored required properties.
    private SortedDictionary<int, List<SpriteRenderer>> spriteRenderers;

    private void Start()
    {
        spriteRenderers = new SortedDictionary<int, List<SpriteRenderer>>();
        CalculateIndexes();
    }

    private void OnEnable()
    {
        RegisterCallbacks();
    }

    private void OnDisable()
    {
        RemoveCallbacks();
    }

    private void CalculateIndexes()
    {
        GetRenderers();

        int index = spriteRenderers.Count;
        foreach (KeyValuePair<int, List<SpriteRenderer>> item in spriteRenderers)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                SpriteRenderer spriteRenderer = item.Value[i];
                spriteRenderer.sortingOrder = index;
            }
            index--;
        }
    }

    private void GetRenderers()
    {
        spriteRenderers.Clear();
        for (int i = 0; i < unitControllers.Count; i++)
        {
            UnitController controller = unitControllers[i];
            GridEntity entity = controller.GetGridEntity();
            SpriteRenderer spriteRenderer = entity.transform.GetChild(0).GetComponent<SpriteRenderer>();

            int x = entity.GetGridPosition().x;
            if (spriteRenderers.ContainsKey(x))
            {
                spriteRenderers[x].Add(spriteRenderer);
            }
            else
            {
                List<SpriteRenderer> renderers = new List<SpriteRenderer>() { spriteRenderer };
                spriteRenderers.Add(x, renderers);
            }
        }
    }

    public void AddUnit(UnitController unitController)
    {
        unitControllers.Add(unitController);
        unitController.OnDeath += OnDeath;
        unitController.OnMove += CalculateIndexes;

        CalculateIndexes();
    }

    private void OnDeath(StackHealth stackHealth)
    {
        UnitController unitController = stackHealth as UnitController;
        unitControllers.Remove(unitController);

        unitController.OnDeath -= OnDeath;
        unitController.OnMove -= CalculateIndexes;
    }

    private void RegisterCallbacks()
    {
        for (int i = 0;i < unitControllers.Count;i++)
        {
            unitControllers[i].OnMove += CalculateIndexes;
            unitControllers[i].OnDeath += OnDeath;
        }
    }

    private void RemoveCallbacks()
    {
        for (int i = 0; i < unitControllers.Count; i++)
        {
            unitControllers[i].OnMove -= CalculateIndexes;
            unitControllers[i].OnDeath -= OnDeath;
        }
    }
}
