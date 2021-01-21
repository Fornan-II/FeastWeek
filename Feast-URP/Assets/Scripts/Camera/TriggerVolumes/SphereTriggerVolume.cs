using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTriggerVolume : BaseTriggerVolume
{
    [SerializeField] private float radius = 1.0f;

    protected override bool IsMainCameraWithin()
    {
        return transform.InverseTransformPoint(MainCamera.RootTransform.position).sqrMagnitude <= radius * radius;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsOverlapping ? Color.green : Color.white;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, radius);
    }
}
