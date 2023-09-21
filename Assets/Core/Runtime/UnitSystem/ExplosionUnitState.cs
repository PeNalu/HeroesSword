using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit States/Explosion State", fileName = "Explosion State")]
public class ExplosionUnitState : ScriptableUnitState
{
    [SerializeField]
    private float damage = 1000f;

    [SerializeField]
    private int range = 2;

    //Stored required properties.
    private GridData floorData;
    private GridData entityData;
    private GridController gridController;
    private Grid grid;

    private List<GridTile> validTiles;
    private List<Vector3Int> validPositions;
    private List<UnitController> toDamage;
    private UnitController unit;

    private Vector3Int startPosition;

    public override void Initialize(GridController gridController, UnitController unitController)
    {
        this.floorData = gridController.GetFloorData();
        this.entityData = gridController.GetEntityData();
        this.gridController = gridController;
        this.grid = gridController.GetGrid();
        this.unit = unitController;

        toDamage = new List<UnitController>();
        validTiles = new List<GridTile>();
        validPositions = new List<Vector3Int>();
        startPosition = grid.WorldToCell(unitController.transform.position);
        startPosition.z = 0;
    }

    protected override void EndState()
    {
        base.EndState();
        for (int i = 0; i < validTiles.Count; i++)
        {
            validTiles[i].Highlight(false, Color.red);
        }
        validTiles.Clear();
        toDamage.Clear();
    }

    protected override void ActionState(Vector3Int gridPosition, Vector3 position)
    {
        base.ActionState(gridPosition, position);
        if (validPositions.Contains(gridPosition))
        {
            for (int i = 0;i < toDamage.Count; i++)
            {
                UnitController unitController = toDamage[i];
                unitController.TakeDamage(damage);
            }

            unit.TakeDamage(damage);
        }
    }

    protected override void EntryState()
    {
        base.EntryState();
        GridTile gridTile = floorData.GetGridObject(startPosition) as GridTile;
        List<GridTile> allTiles = gridController.SearchingAll(gridTile, range);

        for (int i = 0; i < allTiles.Count; i++)
        {
            GridTile tile = allTiles[i];
            Vector3Int tilePos = tile.GetGridPosition();

            if(entityData.TryGetGridEntity(tilePos, out GridObject gridEntity))
            {
                UnitController controller = gridEntity.GetComponent<UnitController>();
                toDamage.Add(controller);
                tile.Highlight(true, new Color(0f, 1f, 0f, 0.5f));
            }
            else
            {
                tile.Highlight(true, new Color(1f, 0f, 0f, 0.5f));
            }

            validPositions.Add(tilePos);
            validTiles.Add(tile);
        }
    }
}
