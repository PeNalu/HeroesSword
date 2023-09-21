using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;

[CreateAssetMenu(menuName = "Unit States/Range Attack State", fileName = "Range Attack State")]
public class RangeAttackUnitState : ScriptableUnitState
{
    private GridData floorData;
    private GridData entityData;
    private GridController gridController;
    private UnitController unitController;
    private Grid grid;

    private List<GridTile> validTiles;

    private Vector3Int startPosition;
    private GameManager gameManager;

    public override void Initialize(GridController gridController, UnitController unitController)
    {
        this.floorData = gridController.GetFloorData();
        this.entityData = gridController.GetEntityData();
        this.gridController = gridController;
        this.unitController = unitController;
        this.grid = gridController.GetGrid();

        gameManager = GameManager.GetRuntimeInstance();
        validTiles = new List<GridTile>();
        startPosition = grid.WorldToCell(unitController.transform.position);
        startPosition.z = 0;
    }

    protected override void ActionState(Vector3Int gridPosition, Vector3 position)
    {
        GridTile gridTile = floorData.GetGridObject(gridPosition) as GridTile;
        if (validTiles.Contains(gridTile)) // && !entityData.IsEmpty(gridPosition)
        {
            GridObject gridObject = entityData.GetGridObject(gridPosition);
            IHealth health = gridObject.GetComponent<IHealth>();
            float damage = Random.Range(unitController.GetMinDamage(), unitController.GetMaxDamage());
            damage *= unitController.GetCurrentStack();
            health.TakeDamage(damage);
            unitController.CanAttack(false);
            OnEndAction?.Invoke();
        }
    }

    protected override void EndState()
    {
        for (int i = 0; i < validTiles.Count; i++)
        {
            validTiles[i].Highlight(false, Color.red);
        }
        validTiles.Clear();
    }

    protected override void EntryState()
    {
        List<Vector3Int> enemyUnits = gameManager.GetUnitsByTeam(GetEnemyTeam()).Select(x => x.GetGridEntity().GetGridPosition()).ToList();
        GridTile startTile = floorData.GetGridObject(startPosition) as GridTile;
        List<Vector3Int> blockTile = gridController.SearchingAll(startTile, 1).Select(x => x.GetGridPosition()).ToList();

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            Vector3Int pos = enemyUnits[i];
            pos.z = 0;
            GridTile tile = floorData.GetGridObject(pos) as GridTile;

            if(!blockTile.Contains(pos))
            {
                tile.Highlight(true, Color.green);
                validTiles.Add(tile);
            }
        }
    }

    public override event System.Action OnEndAction;

    private Team GetEnemyTeam()
    {
        switch (unitController.GetTeam())
        {
            case Team.Red:
                return Team.Blue;
            case Team.Blue:
                return Team.Red;
            default:
                return Team.Red;
        }
    }
}
