using System;
using System.Collections.Generic;
using UnityEngine;

public class SkilletDeadEvent : GroupDeadEvent
{
    [SerializeField]
    private UnitController template;

    [SerializeField]
    private List<float> weakenings;

    //Stored required properties.
    private int index = 0;



    public override void OnGroupDead(int stackSize)
    {
        UnitController unit = Instantiate(template);

    }

    public override event Action OnEnd;
}
