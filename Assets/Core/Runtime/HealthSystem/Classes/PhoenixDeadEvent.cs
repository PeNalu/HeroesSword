using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Dead Event/Phoenix Dead Event", fileName = "Phoenix Dead Event")]
public class PhoenixDeadEvent : DeadEvent
{
    [SerializeField]
    private UnitController template;

    [SerializeField]
    [Range(0f , 1f)]
    private float rebirthChance = 0.5f;

    //Stored required properties.
    private UnitController parent;
    private GridController gridController;

    public override void Initialize(UnitController parent)
    {
        this.parent = parent;
        gridController = GridController.GetRuntimeInstance();
    }

    public override void OnDead(StackHealth stackHealth)
    {
        float value = UnityEngine.Random.Range(0f, 1f);
        if(value < rebirthChance)
        {
            string color = parent.GetTeam() == GameManager.Team.Red ? "red" : "blue";
            Logger.GetRuntimeInstance().Log($"The <color={color}>{parent.gameObject.name}</color> has been reborn!");

            Vector3Int gridPosition = parent.GetGridEntity().GetGridPosition();
            gridController.GetEntityData().ClearOccupiedCell(gridPosition);
            Vector3 pos = gridController.GetGrid().CellToWorld(gridPosition);
            UnitController unit = Instantiate(template, pos, Quaternion.identity);
            unit.Initialize(
                unit.GetMinDamage() * 2,
                unit.GetMaxDamage() * 2,
                (int)(unit.GetMovementPoint() * 2),
                (int)(unit.GetInitiative() * 2),
                (int)(unit.GetHealth() * 2),
                GameManager.Team.Blue
                );

            gridController.GetEntityData().TryOccupiedCell(gridPosition, parent.GetGridEntity());
            GameManager.GetRuntimeInstance().SpawnUnit(unit, gridPosition);
        }
    }
}
