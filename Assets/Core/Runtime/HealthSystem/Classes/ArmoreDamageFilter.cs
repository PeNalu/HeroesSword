using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage Filters/Armore Filter", fileName = "Armore Filter")]
public class ArmoreDamageFilter : DamageFilter
{
    [SerializeField]
    private List<float> damageProtection;

    //Stored required properties.
    private int currentIndex = 0;

    public override float Filter(float damage)
    {
        float result = Mathf.Max(0, damage - (damage * damageProtection[currentIndex]));
        currentIndex++;

        if(currentIndex == damageProtection.Count)
        {
            OnEndFilter?.Invoke(this);
        }

        return result;
    }

    public override event Action<DamageFilter> OnEndFilter;
}
