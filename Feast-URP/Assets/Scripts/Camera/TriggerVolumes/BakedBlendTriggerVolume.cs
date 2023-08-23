using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakedBlendTriggerVolume : BaseTriggerVolume
{
    public Vector3 HalfExtents => halfExtents;

    [SerializeField] protected Vector3 halfExtents = Vector3.one * 0.5f;
    [SerializeField] private Vector3Int probeCount = new Vector3Int(2, 2, 2);

    [SerializeField, HideInInspector] private FloatField3D _probeField;

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

    private float GetValueAtLocalPosition(Vector3 position)
    {
        // Put position in space relative to minimum corner of Cube bounds.
        // After division, should be scaled so that 0 to 1 spans the Cube bounds.
        Vector3 interpolatedIndex = (position + halfExtents);
        interpolatedIndex.x /= 2f * halfExtents.x;
        interpolatedIndex.y /= 2f * halfExtents.y;
        interpolatedIndex.z /= 2f * halfExtents.z;

        interpolatedIndex.x *= probeCount.x - 1;
        int xLow = Mathf.Clamp(Mathf.FloorToInt(interpolatedIndex.x), 0, probeCount.x - 1);
        int xHigh = Mathf.Clamp(Mathf.CeilToInt(interpolatedIndex.x), 0, probeCount.x - 1);
        float xBlend = interpolatedIndex.x % 1;

        interpolatedIndex.y *= probeCount.y - 1;
        int yLow = Mathf.Clamp(Mathf.FloorToInt(interpolatedIndex.y), 0, probeCount.y - 1);
        int yHigh = Mathf.Clamp(Mathf.CeilToInt(interpolatedIndex.y), 0, probeCount.y - 1);
        float yBlend = interpolatedIndex.y % 1;

        interpolatedIndex.z *= probeCount.z - 1;
        int zLow = Mathf.Clamp(Mathf.FloorToInt(interpolatedIndex.z), 0, probeCount.z - 1);
        int zHigh = Mathf.Clamp(Mathf.CeilToInt(interpolatedIndex.z), 0, probeCount.z - 1);
        float zBlend = interpolatedIndex.z % 1;

#if UNITY_EDITOR
        Vector3 globalPosition = transform.TransformPoint(position);
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(xLow, yLow, zLow)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(xHigh, yLow, zLow)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(xLow, yHigh, zLow)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(xHigh, yHigh, zLow)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(xLow, yLow, zHigh)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(xHigh, yLow, zHigh)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(xLow, yHigh, zHigh)));
        Debug.DrawLine(globalPosition, transform.TransformPoint(GetLocalPositionFromIndex(xHigh, yHigh, zHigh)));
#endif

        // Get initial values from field
        float lll = _probeField.GetValue(xLow, yLow, zLow);
        float hll = _probeField.GetValue(xHigh, yLow, zLow);
        float lhl = _probeField.GetValue(xLow, yHigh, zLow);
        float hhl = _probeField.GetValue(xHigh, yHigh, zLow);
        float llh = _probeField.GetValue(xLow, yLow, zHigh);
        float hlh = _probeField.GetValue(xHigh, yLow, zHigh);
        float lhh = _probeField.GetValue(xLow, yHigh, zHigh);
        float hhh = _probeField.GetValue(xHigh, yHigh, zHigh);

        // Start blending using repeated linear interpolation
        // z
        float ll = Mathf.Lerp(lll, llh, zBlend);
        float hl = Mathf.Lerp(hll, hlh, zBlend);
        float lh = Mathf.Lerp(lhl, lhh, zBlend);
        float hh = Mathf.Lerp(hhl, hhh, zBlend);
        // y
        float l = Mathf.Lerp(ll, lh, yBlend);
        float h = Mathf.Lerp(hl, hh, yBlend);
        // x
        return Mathf.Lerp(l, h, zBlend);
    }

    private Vector3 GetLocalPositionFromIndex(int x, int y, int z)
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

    [ContextMenu("Generate test probe field")]
    private void EDITOR_GenerateTestProbeField()
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

    private void OnValidate()
    {
        probeCount = Vector3Int.Max(probeCount, Vector3Int.one);
        halfExtents = Vector3.Max(halfExtents, Vector3.zero);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = IsOverlapping ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);

        for (int x = 0; x < probeCount.x; ++x)
        {
            for (int y = 0; y < probeCount.y; ++y)
            {
                for (int z = 0; z < probeCount.z; ++z)
                {
                    Gizmos.color = Color.Lerp(Color.black, Color.white, _probeField.GetValue(x, y, z));
                    Gizmos.DrawSphere(GetLocalPositionFromIndex(x, y, z), 0.1f);
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
