using UnityEngine;

public class GridObject : MonoBehaviour
{
    public enum TileType
    {
        Floor,
        Entity
    }

    [SerializeField]
    private TextMesh cellPositionField;

    [SerializeField]
    private TileType tileType;

    //Stored required properties.
    private Vector3Int gridPosition;
    private GridController gridController;

    private void Awake()
    {
        gridController = GridController.GetRuntimeInstance(); 
        gridController.TryOccupied(transform.position, this);
    }

    #region [Getter / Setter]
    public void SetGridPosition(Vector3Int gridPosition)
    {
        this.gridPosition = gridPosition;
        this.gridPosition.z = 0;
    }

    public Vector3Int GetGridPosition()
    {
        return gridPosition;
    }

    public GridController GetGridController()
    {
        return gridController;
    }

    public TileType GetTileType()
    {
        return tileType;
    }
    #endregion
}
