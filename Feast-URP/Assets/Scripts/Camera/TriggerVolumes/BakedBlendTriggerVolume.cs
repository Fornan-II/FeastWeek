using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakedBlendTriggerVolume : BaseTriggerVolume
{
    public Vector3 HalfExtents => halfExtents;

    [SerializeField] protected Vector3 halfExtents = Vector3.one * 0.5f;
    [SerializeField] protected Vector3Int probeCount = new Vector3Int(2, 2, 2);

    [SerializeField, HideInInspector] protected FloatField3D _probeField;

#if UNITY_EDITOR
    [Header("DEBUG")]
    [SerializeField] private float gizmoRenderRadius = -1f;
#endif

    protected override bool IsMainCameraWithin()
    {
        Vector3 cameraLocalPosition = transform.InverseTransformPoint(MainCamera.RootTransform.position);
        bool isWithin = Mathf.Abs(cameraLocalPosition.x) < halfExtents.x && Mathf.Abs(cameraLocalPosition.y) < halfExtents.y && Mathf.Abs(cameraLocalPosition.z) < halfExtents.z;

        if(isWithin)
        {
            BlendValue = GetValueAtLocalPosition(cameraLocalPosition);
        }

        return isWithin;
    }

    #region Converting between index and local position
    private void GetNearestIndices(Vector3 localPosition, out Vector3Int low, out Vector3Int high, out Vector3 blend)
    {
        low = -Vector3Int.one;
        high = -Vector3Int.one;
        blend = -Vector3.one;

        // Put position in space relative to minimum corner of Cube bounds.
        // After division, scaled so that 0 to 1 spans the Cube bounds.
        Vector3 interpolatedIndex = (localPosition + halfExtents);
        interpolatedIndex.x /= 2f * halfExtents.x;
        interpolatedIndex.y /= 2f * halfExtents.y;
        interpolatedIndex.z /= 2f * halfExtents.z;

        interpolatedIndex.x *= probeCount.x - 1;
        low.x = Mathf.Clamp(Mathf.FloorToInt(interpolatedIndex.x), 0, probeCount.x - 1);
        high.x = Mathf.Clamp(Mathf.CeilToInt(interpolatedIndex.x), 0, probeCount.x - 1);
        blend.x = interpolatedIndex.x % 1;

        interpolatedIndex.y *= probeCount.y - 1;
        low.y = Mathf.Clamp(Mathf.FloorToInt(interpolatedIndex.y), 0, probeCount.y - 1);
        high.y = Mathf.Clamp(Mathf.CeilToInt(interpolatedIndex.y), 0, probeCount.y - 1);
        blend.y = interpolatedIndex.y % 1;

        interpolatedIndex.z *= probeCount.z - 1;
        low.z = Mathf.Clamp(Mathf.FloorToInt(interpolatedIndex.z), 0, probeCount.z - 1);
        high.z = Mathf.Clamp(Mathf.CeilToInt(interpolatedIndex.z), 0, probeCount.z - 1);
        blend.z = interpolatedIndex.z % 1;
    }

    protected float GetValueAtLocalPosition(Vector3 localPosition)
    {
        GetNearestIndices(localPosition, out Vector3Int low, out Vector3Int high, out Vector3 blend);

#if UNITY_EDITOR
        Vector3 globalPosition = transform.TransformPoint(localPosition);
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(low.x, low.y, low.z)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(high.x, low.y, low.z)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(low.x, high.y, low.z)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(high.x, high.y, low.z)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(low.x, low.y, high.z)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(high.x, low.y, high.z)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(low.x, high.y, high.z)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(high.x, high.y, high.z)));
#endif

        // Get initial values from field
        float lll = _probeField.GetValue(low.x, low.y, low.z);
        float hll = _probeField.GetValue(high.x, low.y, low.z);
        float lhl = _probeField.GetValue(low.x, high.y, low.z);
        float hhl = _probeField.GetValue(high.x, high.y, low.z);
        float llh = _probeField.GetValue(low.x, low.y, high.z);
        float hlh = _probeField.GetValue(high.x, low.y, high.z);
        float lhh = _probeField.GetValue(low.x, high.y, high.z);
        float hhh = _probeField.GetValue(high.x, high.y, high.z);
        
        // Start blending using repeated linear interpolation
        // z
        float ll = Mathf.Lerp(lll, llh, blend.z);
        float hl = Mathf.Lerp(hll, hlh, blend.z);
        float lh = Mathf.Lerp(lhl, lhh, blend.z);
        float hh = Mathf.Lerp(hhl, hhh, blend.z);
        
        // y
        float l = Mathf.Lerp(ll, lh, blend.y);
        float h = Mathf.Lerp(hl, hh, blend.y);
        
        // x
        return Mathf.Lerp(l, h, blend.x);
    }

    protected Vector3Int GetNearestIndexFromLocalPosition(Vector3 localPosition)
    {
        GetNearestIndices(localPosition, out Vector3Int low, out Vector3Int high, out Vector3 blend);

        Vector3Int index = new Vector3Int(
            blend.x < 0.5 ? low.x : high.x,
            blend.y < 0.5 ? low.y : high.y,
            blend.z < 0.5 ? low.z : high.z
            );

        return index;
    }

    protected Vector3 GetLocalPositionFromIndex(Vector3Int index) => GetLocalPositionFromIndex(index.x, index.y, index.z);

    protected Vector3 GetLocalPositionFromIndex(int x, int y, int z)
    {
        Vector3 pos = Vector3.zero;
        
        if (probeCount.x >= 2)
        {
            pos.x = 2f * halfExtents.x * (x / (probeCount.x - 1f)) - halfExtents.x;
        }
        if (probeCount.y >= 2)
        {
            pos.y = 2f * halfExtents.y * (y / (probeCount.y - 1f)) - halfExtents.y;
        }
        if (probeCount.z >= 2)
        {
            pos.z = 2f * halfExtents.z * (z / (probeCount.z - 1f)) - halfExtents.z;
        }

        return pos;
    }
    #endregion

#if UNITY_EDITOR
    // For bakingFunction:
    // Expects a Vector3 global position of the node,
    // Returns the value to bake at that position
    public void EDITOR_GenerateProbeField(System.Func<Vector3, float> bakingFunction)
    {
        if(bakingFunction == null)
        {
            bakingFunction = (_) => 1f;
        }

        _probeField = new FloatField3D(probeCount.x, probeCount.y, probeCount.z);

        for (int x = 0; x < probeCount.x; ++x)
        {
            for (int y = 0; y < probeCount.y; ++y)
            {
                for (int z = 0; z < probeCount.z; ++z)
                {
                    Vector3 position = transform.TransformPoint(GetLocalPositionFromIndex(x, y, z));
                    _probeField.SetValue(x, y, z, bakingFunction(position));
                }
            }
        }
    }

    // For bakingFunction:
    // Expects a Vector3 global position of the node,
    // Expects the current value of the probe at that position
    // Returns the value to bake at that position
    public void EDITOR_ModifyProbeField(System.Func<Vector3, float, float> bakingFunction)
    {
        if (bakingFunction == null || _probeField == null || _probeField.FieldSize == Vector3Int.zero)
        {
            return;
        }

        for (int x = 0; x < probeCount.x; ++x)
        {
            for (int y = 0; y < probeCount.y; ++y)
            {
                for (int z = 0; z < probeCount.z; ++z)
                {
                    Vector3 position = transform.TransformPoint(GetLocalPositionFromIndex(x, y, z));
                    _probeField.SetValue(x, y, z, bakingFunction(position, _probeField.GetValue(x, y, z)));
                }
            }
        }
    }

    [ContextMenu("Generate test probe field")]
    protected virtual void EDITOR_GenerateTestProbeField()
    {
        UnityEditor.Undo.RecordObject(this, "Generate test probe field");
        
        EDITOR_GenerateProbeField(
            (Vector3 pos) =>
            {
                return UnityEngine.Random.value;
                //pos = (pos + halfExtents) * 0.5f;
                //return Mathf.Max(
                //    pos.x / halfExtents.x,
                //    pos.y / halfExtents.y,
                //    pos.z / halfExtents.z
                //    );
            }
            );
    }

    protected virtual void OnValidate()
    {
        probeCount = Vector3Int.Max(probeCount, Vector3Int.one);
        halfExtents = Vector3.Max(halfExtents, Vector3.zero);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (!isActiveAndEnabled || UnityEditor.Selection.activeGameObject != gameObject) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = IsOverlapping ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);

        for (int x = 0; x < probeCount.x; ++x)
        {
            for (int y = 0; y < probeCount.y; ++y)
            {
                for (int z = 0; z < probeCount.z; ++z)
                {
                    Vector3 localPosition = GetLocalPositionFromIndex(x, y, z);
                    Vector3 editorCameraPos = UnityEditor.SceneView.lastActiveSceneView.camera.transform.position;
                    float sqrDist = (transform.TransformPoint(localPosition) - editorCameraPos).sqrMagnitude;
                    if (gizmoRenderRadius < 0f || sqrDist <= gizmoRenderRadius * gizmoRenderRadius)
                    {
                        Gizmos.color = Color.Lerp(Color.black, Color.white, _probeField.GetValue(x, y, z));
                        //Gizmos.color = Color.HSVToRGB(_probeField.GetValue(x, y, z) * 0.9f, 1f, 0.5f);
                        Gizmos.DrawSphere(localPosition, 0.1f);
                    }
                }
            }
        }

        if (IsOverlapping)
        {
            Vector3 cameraLocalPosition = transform.InverseTransformPoint(MainCamera.RootTransform.position);
            Gizmos.color = Color.Lerp(Color.black, Color.white, GetValueAtLocalPosition(cameraLocalPosition));
            Gizmos.DrawSphere(cameraLocalPosition, 0.1f);
        }
    }
#endif
}
