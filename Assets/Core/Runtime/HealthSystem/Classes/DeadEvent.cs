using UnityEngine;

public abstract class DeadEvent : ScriptableObject
{
    public abstract void Initialize(UnitController parent);
    public abstract void OnDead(StackHealth stackHealth);
}
