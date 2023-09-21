using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : Singleton<GridController>
{
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private CellSelector cellSelector; 

    [SerializeField]
    private Transform tileContainer;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private GridData entityData;

    [SerializeField]
    private GridData floorData;

    [SerializeField]
    private LayerMask cullingLayer;

    //Stored requireed components.
    private InputManager inputManager;

    //Stored required properties.

    private void Awake()
    {
        inputManager = InputManager.GetRuntimeInstance();
        GameManager.GetRuntimeInstance().OnGameEnd += OnGameEnd;
    }

    private void Start()
    {
        GridTile[] gridTiles = GetComponentsInChildren<GridTile>();
        for (int i = 0; i < gridTiles.Length; i++)
        {
            GridTile gridTile = gridTiles[i];
            TryOccupied(gridTile.transform.position, gridTile);
        }
    }

    private void Update()
    {
        if (!floorData.IsEmpty(inputManager.GetCurrentGridPosition()))
        {
            cellSelector.Show(grid.CellToWorld(inputManager.GetCurrentGridPosition()), Color.red);
        }
        else
        {
            cellSelector.Hide();
        }
    }

    private void OnGameEnd()
    {
        floorData.ResetData();
        entityData.ResetData();
    }

    private GridData GetRelevantData(GridObject gridEntity)
    {
        return gridEntity.GetTileType() == GridObject.TileType.Floor ? floorData : entityData;
    }

    public List<GridTile> SearchingWalkable(GridTile startTile, int depth)
    {
        startTile.D = 0;
        List<GridTile> toSearch = new List<GridTile>() { startTile };
        List<GridTile> processed = new List<GridTile>();

        while (toSearch.Count > 0)
        {
            GridTile current = toSearch[0];

            processed.Add(current);
            toSearch.Remove(current);

            if (current.D != depth)
            {
                foreach (GridTile neighbor in current.GetNeighbors().Where(t => !processed.Contains(t)))
                {
                    if (!neighbor.IsWolkable()) continue;
                    if (!entityData.IsEmpty(neighbor.GetGridPosition())) continue;

                    bool inSearch = toSearch.Contains(neighbor);

                    if (!inSearch)
                    {
                        neighbor.D = current.D + 1;
                        neighbor.SetConnect(current);
                        toSearch.Add(neighbor);
                    }
                }
            }
        }

        processed.Remove(startTile);
        return processed;
    }

    public List<GridTile> SearchingAll(GridTile startTile, int depth)
    {
        startTile.D = 0;
        List<GridTile> toSearch = new List<GridTile>() { startTile };
        List<GridTile> processed = new List<GridTile>();

        while (toSearch.Count > 0)
        {
            GridTile current = toSearch[0];

            processed.Add(current);
            toSearch.Remove(current);

            if (current.D != depth)
            {
                foreach (GridTile neighbor in current.GetNeighbors().Where(t => !processed.Contains(t)))
                {
                    bool inSearch = toSearch.Contains(neighbor);

                    if (!inSearch)
                    {
                        neighbor.D = current.D + 1;
                        neighbor.SetConnect(current);
                        toSearch.Add(neighbor);
                    }
                }
            }
        }

        processed.Remove(startTile);
        return processed;
    }

    public bool TryOccupied(Vector3 position, GridObject gridObject)
    {
        Vector3Int gridPos = grid.WorldToCell(position);
        GridData relevantData = GetRelevantData(gridObject);

        if (relevantData.TryOccupiedCell(gridPos, gridObject))
        {
            gridObject.SetGridPosition(gridPos);
            return true;
        }
        return false;
    }

    #region [Getter / Setter]
    public CellSelector GetCellSelector() 
    { 
        return cellSelector; 
    }

    public GridData GetFloorData()
    {
        return floorData;
    }

    public GridData GetEntityData()
    {
        return entityData;
    }

    public Grid GetGrid()
    {
        return grid;
    }
    #endregion
}
