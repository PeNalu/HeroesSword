using System;
using UnityEngine;

public class ScriptableUnitState : ScriptableObject, IUnitState
{
    [SerializeField]
    private string stateName;

    public virtual void Initialize(GridController gridController, UnitController unitController) { }

    public void OnAction(Vector3Int gridPosition, Vector3 position)
    {
        ActionState(gridPosition, position);
    }

    public void OnEnd()
    {
        EndState();
    }

    public void OnEntry()
    {
        EntryState();
    }

    public void OnUpdate(Vector3Int gridPosition, Vector3 position)
    {
        UpdateState(gridPosition, position);
    }

    protected virtual void ActionState(Vector3Int gridPosition, Vector3 position) { }

    protected virtual void EndState() { }

    protected virtual void EntryState() { }

    protected virtual void UpdateState(Vector3Int gridPosition, Vector3 position) { }

    public event Action OnEndAction;

    #region [Getter / Setter]
    public string GetStateName()
    {
        return stateName;
    }
    #endregion
}
