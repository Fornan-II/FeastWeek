using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickGrid : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject sourceItem;
    [SerializeField] private Vector2 gridSize = new Vector2(100, 100);
    [SerializeField] private Vector2 gridDelta = new Vector2(.25f, .25f);
    [SerializeField] private bool randomRotation = false;
    [Header("Physics")]
    [SerializeField] private bool doGroundRaycast = false;
    [SerializeField] private float raycastDistance = 10f;

    [HideInInspector, SerializeField] private List<GameObject> gridItems = new List<GameObject>();

    public void GenerateGrid()
    {
        if (!sourceItem)
        {
            Debug.LogWarning("Can not generate grid when sourceItem is null.");
            return;
        }

        ClearGrid();
        for(int x = 0; x < gridSize.x; ++x)
        {
            for(int y = 0; y < gridSize.y; ++y)
            {
                Vector2 gridPos = (new Vector2(x, y) - gridSize * 0.5f) * gridDelta;
                Vector3 worldPos = transform.TransformPoint(Util.XZVector3(gridPos, transform.position.y));

                if (doGroundRaycast)
                {
                    if (Physics.Raycast(worldPos, Vector3.down, out RaycastHit hitInfo, raycastDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                    {
                        worldPos = hitInfo.point;
                    }
                    else
                    {
                        worldPos += Vector3.down * raycastDistance;
                    }
                }

                gridItems.Add(Instantiate(sourceItem, worldPos, transform.rotation * Quaternion.Euler(0, randomRotation ? Random.Range(0,360) : 0, 0), transform));
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        for (int x = 0; x < gridSize.x; ++x)
        {
            for (int y = 0; y < gridSize.y; ++y)
            {
                Vector2 gridPos = (new Vector2(x, y) - gridSize * 0.5f) * gridDelta;
                Vector3 worldPos = transform.TransformPoint(Util.XZVector3(gridPos, transform.position.y));
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(worldPos, 0.1f);

                if(doGroundRaycast)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(worldPos, Vector3.down * raycastDistance);
                }
            }
        }
    }
#endif
}
