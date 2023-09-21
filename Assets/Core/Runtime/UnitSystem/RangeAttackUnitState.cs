using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;

public class RangeAttackUnitState : IUnitState
{
    private GridData floorData;
    private GridData entityData;
    private GridController gridController;
    private UnitController unitController;
    private Grid grid;

    private List<GridTile> validTiles;
    private List<GridTile> invalidTiles;

    private Vector3Int startPosition;
    private GameManager gameManager;

    public RangeAttackUnitState(GridController gridController, UnitController unitController)
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

    public void OnAction(Vector3Int gridPosition, Vector3 position)
    {
        GridTile gridTile = floorData.GetGridObject(gridPosition) as GridTile;
        if (validTiles.Contains(gridTile) && !entityData.IsEmpty(gridPosition))
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
        List<Vector3Int> enemyUnits = gameManager.GetUnitsByTeam(GetEnemyTeam()).Select(x => x.GetGridEntity().GetGridPosition()).ToList();

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            Vector3Int pos = enemyUnits[i];
            pos.z = 0;
            GridTile tile = floorData.GetGridObject(pos) as GridTile;
            tile.Highlight(true, Color.green);
            validTiles.Add(tile);
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
