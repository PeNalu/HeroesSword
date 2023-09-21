using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[DisallowMultipleComponent]
public class InputManager : Singleton<InputManager>
{
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Camera mainCamera;

    //Stored required properties.
    private int uiLayer;
    private Vector3 currentPosition;
    private Vector3Int currentGridPosition;

    private void Awake()
    {
        uiLayer = LayerMask.NameToLayer("UI");
    }

    private void Update()
    {
        currentPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        currentPosition.z = 0;
        currentGridPosition = grid.WorldToCell(currentPosition);
        currentGridPosition.z = 0;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnClick?.Invoke();
        }
    }

    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == uiLayer)
                return true;
        }
        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    public Vector3Int GetWorldToCellPosition(Vector3 position)
    {
        return grid.WorldToCell(position);
    }

    public Vector3 GetCellToWorldPosition(Vector3Int position)
    {
        return grid.CellToWorld(position);
    }

    #region [Event Callbacks]
    public event Action OnClick;
    public event Action OnExit;
    #endregion

    #region [Getter / Setter]
    public Vector3 GetCurrentPosition()
    {
        return currentPosition;
    }

    public Vector3Int GetCurrentGridPosition()
    {
        return currentGridPosition;
    }
    #endregion
}

