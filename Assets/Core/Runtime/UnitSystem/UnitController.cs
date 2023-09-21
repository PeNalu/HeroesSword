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
    private SpriteRenderer teamIndicator;

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

    [SerializeField]
    [Range(0, 1)]
    private float doubleStepChance = 0f;

    [SerializeField]
    [TextArea]
    private string description;

    //Stored required components.
    private GridEntity gridEntity;
    private InputManager inputManager;

    //Stored required properties.
    //private IUnitState state;
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
        if(currentState != null)
        {
            currentState.OnUpdate(inputManager.GetCurrentGridPosition(), inputManager.GetCurrentPosition());
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

    public void Initialize(float minDamage, float maxDamage, int movePoint, int initiative, float health, Team team)
    {
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
        this.movementPoint = movePoint;
        this.initiative = initiative;
        SetTeam(team);
        SetHealth(health);

        startMovementPoint = movementPoint;
        previousMovePoint = startMovementPoint;
        inputManager = InputManager.GetRuntimeInstance();
        gridEntity = GetComponent<GridEntity>();
    }

    public void OnAction()
    {
        if (currentState == null) return;
        currentState.OnAction(inputManager.GetCurrentGridPosition(), inputManager.GetCurrentPosition());
    }

    public void OnExit()
    {
        if (currentState != null)
        {
            currentState.OnEnd();
            currentState.OnEndAction -= OnExit;
        }

        currentState = Instantiate(baseState);
        currentState.Initialize(gridEntity.GetGridController(), this);
        currentState.OnEndAction += OnExit;
    }

    public void EntryState(string stateName)
    {
        if(currentState != null)
        {
            currentState.OnEnd();
            currentState.OnEndAction -= OnExit;
        }

        ScriptableUnitState templater = statesPair[stateName];
        currentState = Instantiate(templater);
        currentState.Initialize(gridEntity.GetGridController(), this);
        currentState.OnEntry();
        currentState.OnEndAction += OnExit;
    }

    public void ResetUnit()
    {
        movementPoint = startMovementPoint;
        canAttack = true;
    }

    #region [Event Action Callback]
    public event Action OnMove;
    public event Action<UnitController> OnChangeTeam;
    #endregion

    #region [Getter / Setter]
    public string GetDescription()
    {
        return description;
    }

    public float GetDoubleStepChance()
    {
        return doubleStepChance;
    }

    public List<ScriptableUnitState> GetUnitStates()
    {
        return states;
    }

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

        switch (team)
        {
            case Team.Red:
                teamIndicator.color = Color.red;
                break;
            case Team.Blue:
                teamIndicator.color = Color.blue;
                break;
        }

        OnChangeTeam?.Invoke(this);
    }

    private int temporaryTurn = -1;
    private Team startTeam;
    public void TemporarySetTeam(Team team, int turnCount)
    {
        startTeam = currentTeam;
        temporaryTurn = turnCount;
        SetTeam(team);
    }

    public void OnEndTurn()
    {
        if(temporaryTurn != -1)
        {
            temporaryTurn--;
            if(temporaryTurn == 0)
            {
                SetTeam(startTeam);
            }
        }
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
