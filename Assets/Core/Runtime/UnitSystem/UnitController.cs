using System;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[RequireComponent(typeof(GridEntity))]
public class UnitController : StackHealth
{
    public enum UnitType
    {
        Range, 
        Melee
    }

    [SerializeField]
    private List<ScriptableUnitState> states;

    [SerializeField]
    private ScriptableUnitState baseState;

    [SerializeField]
    private Team currentTeam;

    [SerializeField]
    private UnitType type;

    [SerializeField]
    private int initiative;

    [SerializeField]
    private float maxDamage;

    [SerializeField]
    private float minDamage;

    [SerializeField]
    private int movementPoint;

    //Stored required components.
    private GridEntity gridEntity;
    private InputManager inputManager;

    //Stored required properties.
    private IUnitState state;
    private ScriptableUnitState currentState;
    private bool isActive = false;
    private int startMovementPoint;
    private int previousMovePoint;
    private bool canAttack = true;
    private Dictionary<string, ScriptableUnitState> statesPair;

    protected override void Awake()
    {
        base.Awake();
        statesPair = new Dictionary<string, ScriptableUnitState>();

        for (int i = 0; i < states.Count; i++)
        {
            ScriptableUnitState state = states[i];
            statesPair.Add(state.GetStateName(), state);
        }

        startMovementPoint = movementPoint;
        previousMovePoint = startMovementPoint;
        inputManager = InputManager.GetRuntimeInstance();
        gridEntity = GetComponent<GridEntity>();
    }

    private void Update()
    {
        if(state != null)
        {
            state.OnUpdate(inputManager.GetCurrentGridPosition(), inputManager.GetCurrentPosition());
        }

        if(movementPoint < previousMovePoint)
        {
            previousMovePoint = movementPoint;
            OnMove?.Invoke();
        }
        else if(movementPoint > previousMovePoint)
        {
            previousMovePoint = startMovementPoint;
        }
    }

    public void OnAction()
    {
        if (state == null) return;
        state.OnAction(inputManager.GetCurrentGridPosition(), inputManager.GetCurrentPosition());
    }

    public void OnExit()
    {
        if(state != null)
        {
            state.OnEnd();
        } 

        state = new BaseUnitState(gridEntity.GetGridController(), this);
    }

    public void EntryState(string stateName)
    {
        ScriptableUnitState templater = statesPair[stateName];
        currentState = Instantiate(templater);
    }

    public void EntryMovementState()
    {
        if (state != null)
        {
            state.OnEnd();
        }

        state = new MovementUnitState(gridEntity.GetGridController(), this);
        state.OnEndAction += OnExit;
        state.OnEntry();
    }

    public void EntryBaseAttackState()
    {
        if (state != null)
        {
            state.OnEnd();
        }

        switch (type)
        {
            case UnitType.Range:
                state = new RangeAttackUnitState(gridEntity.GetGridController(), this);
                break;
            case UnitType.Melee:
                state = new MeleeAttackUnitState(gridEntity.GetGridController(), this);
                break;
        }

        state.OnEndAction += OnExit;
        state.OnEntry();
    }

    public void ResetUnit()
    {
        movementPoint = startMovementPoint;
        canAttack = true;
    }

    #region [Event Action Callback]
    public event Action OnMove;
    #endregion

    #region [Getter / Setter]
    public bool CanAttack()
    {
        return canAttack;
    }

    public void CanAttack(bool value)
    {
        canAttack = value;
    }

    public GridEntity GetGridEntity() 
    { 
        return gridEntity; 
    }

    public UnitType GetUnitType()
    {
        return type;
    }

    public int GetInitiative()
    {
        return initiative;
    }

    public float GetMaxDamage()
    {
        return maxDamage;
    }

    public float GetMinDamage()
    {
        return minDamage;
    }

    public int GetMovementPoint()
    {
        return movementPoint;
    }

    public void SpendMovementPoint(int amount)
    {
        amount = Mathf.Abs(amount);
        movementPoint = Mathf.Max(0, movementPoint - amount);
    }

    public Team GetTeam()
    {
        return currentTeam;
    }

    public void SetTeam(Team team)
    {
        currentTeam = team;
    }

    public void SetActive(bool value)
    {
        isActive = value;
    }

    public bool IsActive()
    {
        return isActive;
    }
    #endregion
}
