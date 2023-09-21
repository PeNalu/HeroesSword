using System.Collections.Generic;
using UnityEngine;

public class GridTile : GridObject
{
    [SerializeField]
    private GridData floorData;

    [SerializeField]
    private GameObject highlightObject;

    [SerializeField]
    private GameObject baseObject;

    [SerializeField]
    private SpriteRenderer highlightRenderer;

    [SerializeField]
    private bool isWolkable = true;

    public float D;
    public float G;

    //Stored required properties.
    private List<GridTile> neighbors;
    private GridTile connect;

    private void Start()
    {
        neighbors = new List<GridTile>();
        List<GridObject> gridObjects = floorData.GetNeighbors(GetGridPosition());

        for (int i = 0; i < gridObjects.Count; i++)
        {
            neighbors.Add(gridObjects[i] as GridTile);
        }
    }

    public void IsWolkable(bool value)
    {
        isWolkable = value;
    }

    public bool IsWolkable()
    {
        return isWolkable;
    }

    public void Highlight(bool value, Color color) 
    {
        highlightRenderer.color = color;
        baseObject.SetActive(!value);
        highlightObject.SetActive(value);
    }

    public List<GridTile> GetNeighbors()
    {
        return neighbors;
    }

    public GridTile GetConnect()
    {
        return connect;
    }

    public void SetConnect(GridTile tile)
    {
        connect = tile;
    }

    public float GetDistance(Vector3Int vector)
    {
        return Vector3Int.Distance(GetGridPosition(), vector);
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < neighbors.Count; i++)
            {
                Gizmos.DrawSphere(neighbors[i].transform.position, 0.2f);
            }
        }
    }
}
