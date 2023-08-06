using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTriggerVolume : BaseTriggerVolume
{
    [SerializeField] private Vector3 halfExtents = Vector3.one * 0.5f;
    [SerializeField] private Vector3 innerHalfExtents = Vector3.one * 0.5f;

    protected override bool IsMainCameraWithin()
    {
        Vector3 cameraLocalPosition = transform.InverseTransformPoint(MainCamera.RootTransform.position);
        cameraLocalPosition.x = Mathf.Abs(cameraLocalPosition.x);
        cameraLocalPosition.y = Mathf.Abs(cameraLocalPosition.y);
        cameraLocalPosition.z = Mathf.Abs(cameraLocalPosition.z);

        BlendValue = Mathf.Min(
            Mathf.Clamp01(Mathf.InverseLerp(halfExtents.x, innerHalfExtents.x, cameraLocalPosition.x)),
            Mathf.Clamp01(Mathf.InverseLerp(halfExtents.y, innerHalfExtents.y, cameraLocalPosition.y))
            );
        BlendValue = Mathf.Min(
            BlendValue,
            Mathf.Clamp01(Mathf.InverseLerp(halfExtents.z, innerHalfExtents.z, cameraLocalPosition.z))
            );
        // Calling Mathf.Min twice with 2 parameters instead of once with 3 parameters because C# params allocates a little bit of garbage.
        // The amount is minor; but considering this is called every frame, it doesn't hurt to avoid allocating unnecessary garbage.

        return cameraLocalPosition.x < halfExtents.x && cameraLocalPosition.y < halfExtents.y && cameraLocalPosition.z < halfExtents.z;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        halfExtents.x = Mathf.Max(halfExtents.x, 0f);
        halfExtents.y = Mathf.Max(halfExtents.y, 0f);
        halfExtents.z = Mathf.Max(halfExtents.z, 0f);
        innerHalfExtents.x = Mathf.Max(innerHalfExtents.x, 0f);
        innerHalfExtents.y = Mathf.Max(innerHalfExtents.y, 0f);
        innerHalfExtents.z = Mathf.Max(innerHalfExtents.z, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsOverlapping ? Color.green : Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
        Gizmos.DrawWireCube(Vector3.zero, innerHalfExtents * 2f);
    }
#endif
}
