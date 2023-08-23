using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTriggerVolume : BaseTriggerVolume
{
    public Vector3 HalfExtents => halfExtents;
    public Vector3 InnerHalfExtents => innerHalfExtents;

    [SerializeField] protected Vector3 halfExtents = Vector3.one * 0.5f;
    [SerializeField] protected Vector3 innerHalfExtents = Vector3.one * 0.5f;
    
    protected float GetCubeBlendValue(Vector3 localPosition)
    {
        localPosition.x = Mathf.Abs(localPosition.x);
        localPosition.y = Mathf.Abs(localPosition.y);
        localPosition.z = Mathf.Abs(localPosition.z);

        // ternary checks to avoid divide by zero edge case
        Vector3 blendVector = new Vector3(
            halfExtents.x != innerHalfExtents.x ? Mathf.InverseLerp(halfExtents.x, innerHalfExtents.x, localPosition.x) : 1f,
            halfExtents.y != innerHalfExtents.y ? Mathf.InverseLerp(halfExtents.y, innerHalfExtents.y, localPosition.y) : 1f,
            halfExtents.z != innerHalfExtents.z ? Mathf.InverseLerp(halfExtents.z, innerHalfExtents.z, localPosition.z) : 1f
            );

        // Calling Mathf.Min twice with 2 parameters instead of once with 3 parameters because C# params allocates a little bit of garbage.
        // The amount is minor; but considering this is called every frame, it doesn't hurt to avoid allocating unnecessary garbage.
        float blendValue = Mathf.Min(blendVector.x, blendVector.y);
        blendValue = Mathf.Min(blendValue, blendVector.z);

        return blendValue;
    }

    protected override bool IsMainCameraWithin()
    {
        Vector3 cameraLocalPosition = transform.InverseTransformPoint(MainCamera.RootTransform.position);

        BlendValue = GetCubeBlendValue(cameraLocalPosition);

        cameraLocalPosition.x = Mathf.Abs(cameraLocalPosition.x);
        cameraLocalPosition.y = Mathf.Abs(cameraLocalPosition.y);
        cameraLocalPosition.z = Mathf.Abs(cameraLocalPosition.z);

        return cameraLocalPosition.x < halfExtents.x && cameraLocalPosition.y < halfExtents.y && cameraLocalPosition.z < halfExtents.z;
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        halfExtents = Vector3.Max(halfExtents, Vector3.zero);
        innerHalfExtents = Vector3.Max(innerHalfExtents, Vector3.zero);
        innerHalfExtents = Vector3.Min(halfExtents, Vector3.zero);
    }

    [SerializeField, Min(0)] private int Editor_BlendGizmoCount = 3;
    protected virtual void OnDrawGizmosSelected()
    {
        if (!isActiveAndEnabled) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        
        if(IsOverlapping)
        {
            Gizmos.color = Color.Lerp(Color.yellow, Color.green, BlendValue);
            Gizmos.DrawWireCube(Vector3.zero, transform.InverseTransformPoint(MainCamera.RootTransform.position) * 2f);
        }

        // Always show bounds
        int divCount = Mathf.Max(2, Editor_BlendGizmoCount);
        Vector3 delta = (halfExtents - innerHalfExtents) / (divCount - 1);
        for (int div = 0; div < divCount; ++div)
        {
            Vector3 offset = innerHalfExtents + delta * div;
            Color col = Color.Lerp(Color.yellow, Color.green, GetCubeBlendValue(offset));
            col.a = (div == 0 || div == divCount - 1) ? 1f : 0.5f;
            Gizmos.color = col;
            Gizmos.DrawWireCube(Vector3.zero, offset * 2f);
        }

        // Earlier debug code for visualizing the actual value distribution for blend.
        //
        //int gridDensity = 3;
        //Vector3 delta = (halfExtents - innerHalfExtents) / (gridDensity - 1);
        //
        //for (float x = 0; x < gridDensity; ++x)
        //{
        //    for (float y = 0; y < gridDensity; ++y)
        //    {
        //        for (float z = 0; z < gridDensity; ++z)
        //        {
        //            Vector3 localDelta = new Vector3(x * delta.x, y * delta.y, z * delta.z);
        //
        //            Vector3 pos = localDelta + halfExtents;
        //            float value = GetCubeBlendValue(pos);
        //            Util.DebugDrawPoint(transform.TransformPoint(pos), new Color(0f, value, value));
        //        }
        //    }
        //}
    }
#endif
}
