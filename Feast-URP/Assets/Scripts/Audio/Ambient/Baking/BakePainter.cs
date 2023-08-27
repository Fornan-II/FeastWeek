using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakePainter : MonoBehaviour
{
#if UNITY_EDITOR
#pragma warning disable 0649
    [SerializeField] private BakedBlendTriggerVolume target;
    [SerializeField, Range(0f, 1f)] private float gizmoAlpha = 0.5f;
    [SerializeField, Range(0f, 1f)] private float value = 1f;
    [SerializeField, Min(0f)] private float feather = 0f;
    [SerializeField, Min(0f)] private float radius = 0.5f;

    [ContextMenu("Paint")]
    private void Paint()
    {
        if (!target) return;

        target.EDITOR_ModifyProbeField((Vector3 position, float currentValue) =>
        {
            position = transform.InverseTransformPoint(position);

            if (feather > 0f)
            {
                float alpha = Mathf.Clamp01(Mathf.InverseLerp(radius + feather, radius, position.magnitude));
                return Mathf.Lerp(value, currentValue, alpha);
            }

            return position.sqrMagnitude <= radius * radius ? value : currentValue;
        });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Color col = Color.white * value;
        col.a = gizmoAlpha;
        Gizmos.color = col;
        Gizmos.DrawSphere(Vector3.zero, radius);

        if (feather > 0f)
        {
            Gizmos.DrawWireSphere(Vector3.zero, radius + feather);
        }
    }
#endif
}
