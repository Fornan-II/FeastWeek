using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickGrid : MonoBehaviour
{
    [SerializeField] private GameObject sourceItem;
    [SerializeField] private Vector2 gridSize = new Vector2(100, 100);
    [SerializeField] private Vector2 gridDelta = new Vector2(.25f, .25f);
    [SerializeField] private bool randomRotation = false;

    [HideInInspector, SerializeField] private List<GameObject> gridItems = new List<GameObject>();

    public void GenerateGrid()
    {
        ClearGrid();
        for(int x = 0; x < gridSize.x; ++x)
        {
            for(int y = 0; y < gridSize.y; ++y)
            {
                Vector2 gridPos = (new Vector2(x, y) - gridSize * 0.5f) * gridDelta;
                gridItems.Add(Instantiate(sourceItem, transform.TransformPoint(Util.XZVector3(gridPos)), transform.rotation * Quaternion.Euler(0, Random.Range(0,360), 0), transform));
            }
        }
    }

    public void ClearGrid()
    {
        while(gridItems.Count > 0)
        {
            DestroyImmediate(gridItems[0]);
            gridItems.RemoveAt(0);
        }
    }
}
