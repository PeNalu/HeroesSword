using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
    public enum Team
    {
        Red, 
        Blue
    }

    [SerializeField]
    private CharacterStatsPanel statsPanel;

    [SerializeField]
    private GridData floorData;

    [SerializeField]
    private WinPanel winPanel;

    [SerializeField]
    private List<UnitController> blueTeam;

    [SerializeField]
    private List<UnitController> redTeam;

    //Stored required properties.
    private List<UnitController> allUnits;
    private UnitController currentActiveUnit;
    private InputManager inputManager;
    private GridTile selectedTile;
    private int unitIndex = 0;

    private void Awake()
    {
        inputManager = InputManager.GetRuntimeInstance();
        inputManager.OnClick += OnClick;
        inputManager.OnExit += OnExit;
        allUnits = new List<UnitController>(blueTeam);

        for (int i = 0; i < redTeam.Count; i++) 
        {
            allUnits.Add(redTeam[i]);
        }

        for (int i = 0; i < allUnits.Count; i++)
        {
            allUnits[i].OnDeath += OnDeath;
        }

        OrderUnits();
    }

    private void Start()
    {
        currentActiveUnit = allUnits[0];
        statsPanel.SetCharacter(currentActiveUnit);
        currentActiveUnit.OnMove += HighlightSelected;
        HighlightSelected();
    }

    private void Update()
    {
        CheckWinner();
    }

    public void EntryMovementState()
    {
        currentActiveUnit.EntryMovementState();
    }

    public void EntryBaseAttackState()
    {
        currentActiveUnit.EntryBaseAttackState();
    }

    private void OnClick()
    {
        currentActiveUnit.OnAction();
    }

    private void OnExit()
    {
        currentActiveUnit.OnExit();
    }

    private void CheckWinner()
    {
        if (redTeam.Count == 0)
        {
            winPanel.Show("Team Blue Win");
            OnGameEnd?.Invoke();
        }

        if (blueTeam.Count == 0)
        {
            winPanel.Show("Team Red Win");
            OnGameEnd?.Invoke();
        }
    }

    private void OnDeath(StackHealth stackHealth)
    {
        UnitController controller = stackHealth as UnitController;
        if (controller != null)
        {
            if(controller.GetTeam() == Team.Blue)
            {
                blueTeam.Remove(controller);
            }
            else 
            {
                redTeam.Remove(controller);
            }
            allUnits.Remove(controller);
            Destroy(controller.gameObject);
        }
    }

    public void EndTurn()
    {
        unitIndex++;
        if (unitIndex > allUnits.Count - 1)
        {
            unitIndex = 0;
            ResetUnits();
        }

        currentActiveUnit.OnExit();
        currentActiveUnit.OnMove -= HighlightSelected;
        currentActiveUnit = allUnits[unitIndex];
        statsPanel.SetCharacter(currentActiveUnit);
        currentActiveUnit.OnMove += HighlightSelected;
        HighlightSelected();
    }

    private void HighlightSelected()
    {
        if (selectedTile != null)
        {
            selectedTile.Highlight(false, Color.red);
        }

        Vector3Int pos = currentActiveUnit.GetGridEntity().GetGridPosition();
        selectedTile = floorData.GetGridObject(pos) as GridTile;
        selectedTile.Highlight(true, Color.yellow);
    }

    private void ResetUnits()
    {
        OnRoundEnd?.Invoke();
        for (int i = 0;i < allUnits.Count;i++)
        {
            allUnits[i].ResetUnit();
        }
    }

    private void OrderUnits()
    {
        allUnits.OrderBy(x => x.GetInitiative());
        allUnits.Reverse();
    }

    #region [Getter / Setter]
    public void SetTeam(UnitController unitController, Team team)
    {
        if (unitController.GetTeam() == Team.Blue)
        {
            blueTeam.Remove(unitController);
            redTeam.Add(unitController);
        }
        else
        {
            redTeam.Remove(unitController);
            blueTeam.Add(unitController);
        }

        unitController.SetTeam(team);
    }

    public List<UnitController> GetUnitsByTeam(Team team)
    {
        switch (team)
        {
            case Team.Red:
                return redTeam;
            case Team.Blue:
                return blueTeam;
            default:
                return null;
        }
    }

    public UnitController GetActiveUnit()
    {
        return currentActiveUnit;
    }
    #endregion

    #region [Event Callback Functions]
    public event Action OnGameEnd;
    public event Action OnRoundEnd;
    #endregion
}
