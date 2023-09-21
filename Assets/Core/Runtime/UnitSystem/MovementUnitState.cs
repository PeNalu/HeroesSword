using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit States/Movement State", fileName = "Movement State")]
public class MovementUnitState : ScriptableUnitState
{
    private GridData floorData;
    private GridData entityData;
    private GridController gridController;
    private UnitController unitController;
    private Grid grid;

    private Dictionary<Vector3Int, GridTile> validTiles;
    private Vector3Int startPosition;

    public override void Initialize(GridController gridController, UnitController unitController)
    {
        this.floorData = gridController.GetFloorData();
        this.entityData = gridController.GetEntityData();
        this.gridController = gridController;
        this.unitController = unitController;
        this.grid = gridController.GetGrid();

        validTiles = new Dictionary<Vector3Int, GridTile>();
        startPosition = grid.WorldToCell(unitController.transform.position);
        startPosition.z = 0;
    }

    protected override void EndState()
    {
        foreach (GridTile tile in validTiles.Values)
        {
            tile.Highlight(false, Color.red);
        }
        validTiles.Clear();
    }

    protected override void EntryState()
    {
        CalculateMovementArea();
        foreach (GridTile tile in validTiles.Values)
        {
            tile.Highlight(true, Color.green);
        }
    }

    protected override void ActionState(Vector3Int gridPosition, Vector3 position)
    {
        if (validTiles.ContainsKey(gridPosition))
        {
            unitController.transform.position = grid.CellToWorld(gridPosition);
            GridTile gridTile = floorData.GetGridObject(gridPosition) as GridTile;
            entityData.ClearOccupiedCell(unitController.GetGridEntity().GetGridPosition());

            if(entityData.TryOccupiedCell(gridPosition, unitController.GetGridEntity()))
            {
                GridTile tile = floorData.GetGridObject(unitController.GetGridEntity().GetGridPosition()) as GridTile;
                tile.IsWolkable(true);
            }

            unitController.SpendMovementPoint((int)gridTile.D);
            unitController.GetGridEntity().SetGridPosition(gridPosition);
            OnEndAction?.Invoke();
        }
    }

    public override event System.Action OnEndAction;

    private void CalculateMovementArea()
    {
        GridTile gridTile = floorData.GetGridObject(startPosition) as GridTile;
        List<GridTile> tiles = gridController.SearchingWalkable(gridTile, unitController.GetMovementPoint());
        for (int i = 0; i < tiles.Count; i++)
        {
            GridTile tile = tiles[i];
            if (entityData.IsEmpty(tile.GetGridPosition()))
            {
                validTiles.Add(tile.GetGridPosition(), tile);
            }
        }
    }
}
