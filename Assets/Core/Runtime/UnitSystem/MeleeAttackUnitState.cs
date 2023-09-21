using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;

[CreateAssetMenu(menuName = "Unit States/Melee Attack State", fileName = "Melee Attack State")]
public class MeleeAttackUnitState : ScriptableUnitState
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
        GridTile gridTile = floorData.GetGridObject(startPosition) as GridTile;
        List<GridTile> allTiles = gridController.SearchingAll(gridTile, 1);
        List<Vector3Int> enemyUnits = gameManager.GetUnitsByTeam(GetEnemyTeam()).Select(x => x.GetGridEntity().GetGridPosition()).ToList();

        for (int i = 0;i < allTiles.Count;i++)
        {
            GridTile tile = allTiles[i];
            if (enemyUnits.Contains(tile.GetGridPosition()))
            {
                tile.Highlight(true, new Color(0f, 1f, 0f, 0.5f));
            }
            else
            {
                tile.Highlight(true, new Color(1f, 0f, 0f, 0.5f));
            }
            validTiles.Add(tile);
        }
    }

    protected override void ActionState(Vector3Int gridPosition, Vector3 position)
    {
        if (entityData.TryGetGridEntity(gridPosition, out GridObject gridEntity))
        {
            UnitController unitController = gridEntity.GetComponent<UnitController>();
            if (unitController == null || unitController.GetTeam() == this.unitController.GetTeam()) return;

            IHealth health = gridEntity.GetComponent<IHealth>();
            float damage = Random.Range(unitController.GetMinDamage(), unitController.GetMaxDamage());
            damage *= unitController.GetCurrentStack();
            health.TakeDamage(damage);
            this.unitController.CanAttack(false);
            OnEndAction?.Invoke();
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
