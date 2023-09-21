using System;
using UnityEngine;

public abstract class GroupDeadEvent : ScriptableObject
{
    public abstract void Initialize(UnitController parent);
    public abstract void OnGroupDead(int stackSize);

    public abstract event Action<GroupDeadEvent> OnEnd;
}
