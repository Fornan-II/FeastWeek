using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTriggerVolume : BaseTriggerVolume
{
    [SerializeField] private float radius = 1.0f;
    [SerializeField] private float innerRadius = 1.0f;

    /// <summary>
    /// A 0 to 1 value indicating camera's position between radii. Set during Unity Update() call before overlap calls.
    /// </summary>
    protected float blendValue;

    protected override bool IsMainCameraWithin()
    {
        float distanceToCamera = transform.InverseTransformPoint(MainCamera.RootTransform.position).magnitude;
        blendValue = Mathf.Clamp01(Mathf.InverseLerp(radius, innerRadius, distanceToCamera));
        return distanceToCamera <= radius;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (innerRadius > radius)
            innerRadius = radius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsOverlapping ? Color.green : Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, radius);
        Gizmos.DrawWireSphere(Vector3.zero, innerRadius);
    }
#endif
}
