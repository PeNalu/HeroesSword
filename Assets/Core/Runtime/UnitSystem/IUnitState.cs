using System;
using UnityEngine;

public interface IUnitState
{
    void OnEnd();
    void OnEntry();
    void OnAction(Vector3Int gridPosition, Vector3 position);
    void OnUpdate(Vector3Int gridPosition, Vector3 position);

    event Action OnEndAction;
}
