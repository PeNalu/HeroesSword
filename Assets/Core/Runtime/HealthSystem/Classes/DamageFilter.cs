using System;
using UnityEngine;

public abstract class DamageFilter : ScriptableObject
{
    public abstract float Filter(float damage);

    public abstract event Action<DamageFilter> OnEndFilter;
}
