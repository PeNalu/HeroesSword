using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit States/Base State", fileName = "Base State")]
public class BaseUnitState : ScriptableUnitState
{
    private GridData floorData;
    private CellSelector cellSelector;  
    private Grid grid;

    private List<GridTile> validTiles;
    private Vector3Int startPosition;

    public override void Initialize(GridController gridController, UnitController unitController) 
    {
        this.floorData = gridController.GetFloorData();
        this.grid = gridController.GetGrid();
        this.cellSelector = gridController.GetCellSelector();

        validTiles = new List<GridTile>();
        startPosition = grid.WorldToCell(unitController.transform.position);
        startPosition.z = 0;
    }

    protected override void UpdateState(Vector3Int gridPosition, Vector3 position)
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
}
