using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovementUnitState : IUnitState
{
    private GridData floorData;
    private GridData entityData;
    private GridController gridController;
    private UnitController unitController;
    private Grid grid;

    private Dictionary<Vector3Int, GridTile> validTiles;
    private Vector3Int startPosition;

    public MovementUnitState(GridController gridController, UnitController unitController)
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

    public void OnEnd()
    {
        foreach (GridTile tile in validTiles.Values)
        {
            tile.Highlight(false, Color.red);
        }
        validTiles.Clear();
    }

    public void OnEntry()
    {
        CalculateMovementArea();
        foreach (GridTile tile in validTiles.Values)
        {
            tile.Highlight(true, Color.green);
        }
    }

    public void OnAction(Vector3Int gridPosition, Vector3 position)
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
                Debug.Log("Move");
            }

            unitController.SpendMovementPoint((int)gridTile.D);
            unitController.GetGridEntity().SetGridPosition(gridPosition);
            OnEndAction?.Invoke();
        }
    }

    public void OnUpdate(Vector3Int gridPosition, Vector3 position)
    {
        
    }

    public event System.Action OnEndAction;

    private void CalculateMovementArea()
    {
        GridTile gridTile = floorData.GetGridObject(startPosition) as GridTile;
        List<GridTile> tiles = gridController.Searching(gridTile, unitController.GetMovementPoint());
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
