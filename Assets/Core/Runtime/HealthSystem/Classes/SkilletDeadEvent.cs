using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grounp Dead Event/Skillet Dead", fileName = "Skillet Dead")]
public class SkilletDeadEvent : GroupDeadEvent
{
    [SerializeField]
    private UnitController template;

    [SerializeField]
    private List<float> weakenings;

    //Stored required properties.
    private UnitController parent;
    private GridController gridController;
    private int index = 0;

    public override void Initialize(UnitController parent)
    {
        this.parent = parent;
        gridController = GridController.GetRuntimeInstance();
    }

    public override void OnGroupDead(int stackSize)
    {
        float weakening = weakenings[index];
        if(TryGetValidPosition(out Vector3Int validGridPosition))
        {
            Vector3 pos = gridController.GetGrid().CellToWorld(validGridPosition);
            UnitController unit = Instantiate(template, pos, Quaternion.identity);
            float percent = (1 - weakening);
            unit.Initialize(
                Math.Max(1, (int)(unit.GetMinDamage() * percent)),
                Math.Max(1, (int)(unit.GetMaxDamage() * percent)),
                Math.Max(1, (int)(unit.GetMovementPoint() * percent)),
                Math.Max(1, (int)(unit.GetInitiative() * percent)),
                Math.Max(1, (int)(unit.GetHealth() * percent)),
                GameManager.Team.Blue
                );

            GameManager.GetRuntimeInstance().SpawnUnit(unit, validGridPosition);
        }
        index++;
        if(index == weakenings.Count)
        {
            OnEnd?.Invoke(this);
        }
    }

    private bool TryGetValidPosition(out Vector3Int validPosition)
    {
        GridTile tile = gridController.GetFloorData().GetGridObject(parent.GetGridEntity().GetGridPosition()) as GridTile;
        List<GridTile> tiles = gridController.SearchingWalkable(tile, 1);

        if(tiles.Count != 0)
        {
            validPosition = tiles[UnityEngine.Random.Range(0, tiles.Count)].GetGridPosition();
            return true;
        }

        validPosition = Vector3Int.zero;
        return false;
    }

    public override event Action<GroupDeadEvent> OnEnd;
}
