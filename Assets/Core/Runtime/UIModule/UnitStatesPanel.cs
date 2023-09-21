using System.Collections.Generic;
using UnityEngine;

public class UnitStatesPanel : MonoBehaviour
{
    //Stored required components.
    private UnitController controller;

    public void Initialize(UnitController unitController)
    {
        controller = unitController;
        Fill();
    }

    private void Fill()
    {
        List<ScriptableUnitState> states = controller.GetUnitStates();
        for (int i = 0; i < states.Count; i++)
        {
            UnitStateSlot slot = Instantiate(states[i].GetSlotTemplate(), transform);
            slot.Initialize(states[i].GetStateName());
            slot.OnClickCallback += OnClick;
        }
    }

    public void Clear()
    {
        for (int i = transform.childCount; i > 0; i--)
        {
            Transform transformChild = transform.GetChild(i - 1);
            DestroyImmediate(transformChild.gameObject);
        }
    }

    private void OnClick(string stateName)
    {
        GameManager.GetRuntimeInstance().ChoiceState(stateName);
    }
}
