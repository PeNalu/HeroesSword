using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;

public class MeleeAttackUnitState : IUnitState
{
    private GridData floorData;
    private GridData entityData;
    private GridController gridController;
    private UnitController unitController;
    private Grid grid;

    private List<GridTile> validTiles;
    private Vector3Int startPosition;
    private GameManager gameManager;

    public MeleeAttackUnitState(GridController gridController, UnitController unitController)
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

    public void OnEnd()
    {
        for (int i = 0; i < validTiles.Count; i++)
        {
            validTiles[i].Highlight(false, Color.red);
        }
        validTiles.Clear();
    }

    public void OnEntry()
    {
        GridTile gridTile = floorData.GetGridObject(startPosition) as GridTile;
        List<GridTile> allTiles = gridController.Searching(gridTile, 1);
        List<Vector3Int> enemyUnits = gameManager.GetUnitsByTeam(GetEnemyTeam()).Select(x => x.GetGridEntity().GetGridPosition()).ToList();

        for (int i = 0;i < allTiles.Count;i++)
        {
            GridTile tile = allTiles[i];
            if (enemyUnits.Contains(tile.GetGridPosition()))
            {
                tile.Highlight(true, Color.green);
            }
            else
            {
                tile.Highlight(true, Color.red);
            }
            validTiles.Add(tile);
        }
    }

    public void OnAction(Vector3Int gridPosition, Vector3 position)
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

    public void OnUpdate(Vector3Int gridPosition, Vector3 position)
    {
        
    }

    public event System.Action OnEndAction;

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
