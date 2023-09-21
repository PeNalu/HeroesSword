using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseUnitState : IUnitState
{
    private GridData floorData;
    private GridData entityData;
    private GridController gridController;
    private UnitController unitController;
    private CellSelector cellSelector;  
    private Grid grid;

    private List<GridTile> validTiles;
    private Vector3Int startPosition;

    public BaseUnitState(GridController gridController, UnitController unitController)
    {
        this.floorData = gridController.GetFloorData();
        this.entityData = gridController.GetEntityData();
        this.gridController = gridController;
        this.unitController = unitController;
        this.grid = gridController.GetGrid();
        this.cellSelector = gridController.GetCellSelector();

        validTiles = new List<GridTile>();
        startPosition = grid.WorldToCell(unitController.transform.position);
        startPosition.z = 0;
    }

    public void OnEnd()
    {
        cellSelector.Hide();
    }

    public void OnEntry()
    {
    }

    public void OnAction(Vector3Int gridPosition, Vector3 position)
    {
    }

    public void OnUpdate(Vector3Int gridPosition, Vector3 position)
    {
        if (!floorData.IsEmpty(gridPosition))
        {
            cellSelector.Show(grid.CellToWorld(gridPosition), Color.red);
        }
        else
        {
            cellSelector.Hide();
        }
    }

    public event System.Action OnEndAction;
}
