using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit States/Arrows Rain State", fileName = "Arrows Rain State")]
public class ArrowsRainUnitState : ScriptableUnitState
{
    private GridData floorData;
    private GridData entityData;
    private GridController gridController;
    private UnitController unitController;
    private Grid grid;

    private List<GridTile> validTiles;
    private List<GridTile> enemyTiles;
    private Vector3Int startPosition;

    public override void Initialize(GridController gridController, UnitController unitController)
    {
        this.floorData = gridController.GetFloorData();
        this.entityData = gridController.GetEntityData();
        this.gridController = gridController;
        this.unitController = unitController;
        this.grid = gridController.GetGrid();

        validTiles = new List<GridTile>();
        enemyTiles = new List<GridTile>();
        startPosition = grid.WorldToCell(unitController.transform.position);
        startPosition.z = 0;
    }

    protected override void EndState()
    {
        base.EndState();
        Clear();
    }

    protected override void ActionState(Vector3Int gridPosition, Vector3 position)
    {
        base.ActionState(gridPosition, position);

        if (enemyTiles.Count == 0)
        {
            return;
        }

        GridTile gridTile = floorData.GetGridObject(gridPosition) as GridTile;
        if (validTiles.Contains(gridTile) && !entityData.IsEmpty(gridPosition))
        {
            float damage = Random.Range(unitController.GetMinDamage(), unitController.GetMaxDamage());
            damage *= unitController.GetCurrentStack();
            damage /= 2;

            for (int i = 0; i < enemyTiles.Count; i++)
            {
                GridObject gridObject = entityData.GetGridObject(enemyTiles[i].GetGridPosition());
                IHealth health = gridObject.GetComponent<IHealth>();
                health.TakeDamage(damage);
            }

            unitController.CanAttack(false);
            OnEndAction?.Invoke();
        }
    }

    protected override void UpdateState(Vector3Int gridPosition, Vector3 position)
    {
        base.UpdateState(gridPosition, position);
        Clear();
        validTiles = CalculateMovementArea(gridPosition);
    }

    private void Clear()
    {
        for (int i = 0; i < validTiles.Count; i++)
        {
            validTiles[i].Highlight(false, Color.red);
        }
        validTiles.Clear();
        enemyTiles.Clear();
    }

    private List<GridTile> CalculateMovementArea(Vector3Int gridPosition)
    {
        List<GridTile> result = new List<GridTile>();
        if (!floorData.ContainsPosition(gridPosition)) return result;

        GridTile startTile = floorData.GetGridObject(startPosition) as GridTile;
        List<GridTile> blockTile = gridController.SearchingAll(startTile, 1);

        Vector3Int pos1 = gridPosition + new Vector3Int(1, 0, 0);
        Vector3Int pos2 = gridPosition + new Vector3Int(-1, 0, 0);

        List<GridTile> tiles1 = new List<GridTile>();
        List<GridTile> tiles2 = new List<GridTile>();

        if (floorData.ContainsPosition(pos1))
        {
            GridTile gridTile1 = floorData.GetGridObject(pos1) as GridTile;

            if(!blockTile.Contains(gridTile1))
                result.Add(gridTile1);
            if (gridTile1 != null)
                tiles1 = gridController.SearchingAll(gridTile1, 1);
        }

        if (floorData.ContainsPosition(pos2))
        {
            GridTile gridTile2 = floorData.GetGridObject(pos2) as GridTile;

            if (!blockTile.Contains(gridTile2))
                result.Add(gridTile2);
            if (gridTile2 != null)
                tiles2 = gridController.SearchingAll(gridTile2, 1);
        }

        for (int i = 0; i < tiles1.Count; i++)
        {
            GridTile tile = tiles1[i];
            if (!blockTile.Contains(tile))
                result.Add(tile);
        }

        for (int i = 0; i < tiles2.Count; i++)
        {
            GridTile tile = tiles2[i];
            if (!result.Contains(tile))
            {
                if (!blockTile.Contains(tile))
                    result.Add(tile);
            }
        }

        for (int i = 0; i < result.Count; i++)
        {
            GridTile tile = result[i];
            if (!entityData.IsEmpty(tile.GetGridPosition()))
            {
                UnitController unit = entityData.GetGridObject(tile.GetGridPosition()).GetComponent<UnitController>();
                if(unit != null && unitController.GetTeam() != unit.GetTeam())
                {
                    enemyTiles.Add(tile);
                    tile.Highlight(true, Color.green);
                }
            }
            else
            {
                tile.Highlight(true, Color.red);
            }
        }

        return result;
    }

    public override event System.Action OnEndAction;
}
