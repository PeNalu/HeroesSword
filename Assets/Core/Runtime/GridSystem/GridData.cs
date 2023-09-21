using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grid/Grid Data", fileName = "Grid Data")]
public class GridData : ScriptableObject
{
    //Stored required properties.
    private Dictionary<Vector3Int, GridObject> cells = new Dictionary<Vector3Int, GridObject>();

    public void ResetData()
    {
        cells.Clear();
    }

    public bool IsEmpty(Vector3Int gridPosition)
    {
        if (!cells.ContainsKey(gridPosition) || cells[gridPosition] == null)
        {
            return true;
        }

        return false;
    }

    public void ClearOccupiedCell(Vector3Int gridPosition)
    {
        if (cells.ContainsKey(gridPosition))
        {
            cells[gridPosition] = null;
        }
    }

    public bool TryOccupiedCell(Vector3Int gridPosition, GridObject gridEntity)
    {
        if (IsEmpty(gridPosition))
        {
            OccupiedCell(gridPosition, gridEntity);
            return true;
        }

        return false;
    }

    public bool TryGetGridEntity(Vector3Int gridPosition, out GridObject gridEntity)
    {
        if (cells.ContainsKey(gridPosition) && cells[gridPosition] != null)
        {
            gridEntity = GetGridObject(gridPosition);
            return true;
        }

        gridEntity = null;
        return false;
    }

    public GridObject GetGridObject(Vector3Int gridPosition)
    {
        if (cells.ContainsKey(gridPosition))
        {
            return cells[gridPosition];
        }
        return null;
    }

    public List<GridObject> GetNeighbors(Vector3Int gridPosition)
    {
        List<GridObject> result = new List<GridObject>();

        foreach (Vector3Int nPos in GetNeighborsOffsets(gridPosition))
        {
            Vector3Int pos = gridPosition + nPos;
            if (cells.ContainsKey(pos))
            {
                result.Add(cells[pos]);
            }
        }

        return result;
    }

    public IEnumerable<Vector3Int> GetNeighborsOffsets(Vector3Int gridPosition)
    {
        bool even = gridPosition.y % 2 == 0;

        // Top
        yield return new Vector3Int(even ? -1 : 0, 1, 0);
        yield return new Vector3Int(even ? 0 : 1, 1, 0);

        // Bottom
        yield return new Vector3Int(even ? -1 : 0, -1, 0);
        yield return new Vector3Int(even ? 0 : 1, -1, 0);

        //Right
        yield return new Vector3Int(1, 0, 0);

        //Left
        yield return new Vector3Int(-1, 0, 0);
    }

    private void OccupiedCell(Vector3Int gridPosition, GridObject gridEntity)
    {
        if (cells.ContainsKey(gridPosition))
        {
            cells[gridPosition] = gridEntity;
        }
        else
        {
            gridPosition.z = 0;
            cells.Add(gridPosition, gridEntity);    
        }
    }
}
