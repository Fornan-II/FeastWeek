using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderTriggerVolume : BaseTriggerVolume
{
    public float Radius => radius;
    public float Height => height;
    public float InnerRadius => innerRadius;
    public float InnerHeight => innerHeight;

    [SerializeField, Min(0f)] private float radius = 1f;
    [SerializeField, Min(0f)] private float height = 1f;
    [SerializeField, Min(0f)] private float innerRadius = 0.5f;
    [SerializeField, Min(0f)] private float innerHeight = 0.5f;
    
    // Based off of Vertical Capped Cylinder; thanks Inigo!
    // https://iquilezles.org/articles/distfunctions/
    protected override bool IsMainCameraWithin()
    {
        Vector3 cameraPos = transform.InverseTransformPoint(MainCamera.RootTransform.position);

        float radiusBlend = Util.GetXZPosition(cameraPos).magnitude;
        float heightBlend = Mathf.Abs(cameraPos.y);
        bool isWithin = radiusBlend <= radius && heightBlend <= height * 0.5f;

        radiusBlend = Mathf.InverseLerp(radius, innerRadius, radiusBlend);
        heightBlend = Mathf.InverseLerp(height * 0.5f, innerHeight * 0.5f, heightBlend);
        BlendValue = Mathf.Min(radiusBlend, heightBlend);

        return isWithin;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (innerRadius > radius)
            innerRadius = radius;
        if (innerHeight > height)
            innerHeight = height;
    }

    private void DrawCylinderGizmo(float r, float h, Color c)
    {
        Color oldGizmoColor = Gizmos.color;
        Matrix4x4 oldGizmoMatrix = Gizmos.matrix;
        Color oldHandleColor = UnityEditor.Handles.color;
        Matrix4x4 oldHandleMatrix = UnityEditor.Handles.matrix;

        UnityEditor.Handles.color = Gizmos.color = c;
        UnityEditor.Handles.matrix = Gizmos.matrix = transform.localToWorldMatrix;

        float halfHeight = h * 0.5f;

        UnityEditor.Handles.DrawWireDisc(Vector3.up * halfHeight, Vector3.up, r);
        UnityEditor.Handles.DrawWireDisc(Vector3.down * halfHeight, Vector3.up, r);
        Gizmos.DrawLine(new Vector3(-r, -halfHeight, 0f), new Vector3(-r, halfHeight, 0f));
        Gizmos.DrawLine(new Vector3(r, -halfHeight, 0f), new Vector3(r, halfHeight, 0f));
        Gizmos.DrawLine(new Vector3(0f, -halfHeight, -r), new Vector3(0f, halfHeight, -r));
        Gizmos.DrawLine(new Vector3(0f, -halfHeight, r), new Vector3(0f, halfHeight, r));

        Gizmos.color = oldGizmoColor;
        Gizmos.matrix = oldGizmoMatrix;
        UnityEditor.Handles.color = oldHandleColor;
        UnityEditor.Handles.matrix = oldHandleMatrix;
    }

    private void OnDrawGizmosSelected()
    {
        if (!isActiveAndEnabled) return;

        DrawCylinderGizmo(radius, height, IsOverlapping ? Color.green : Color.yellow);
        DrawCylinderGizmo(innerRadius, innerHeight, IsOverlapping ? Color.green : Color.yellow);

        if(IsOverlapping)
        {
            Vector3 pos = transform.InverseTransformPoint(MainCamera.RootTransform.position);
            DrawCylinderGizmo(
                Util.GetXZPosition(pos).magnitude,
                Mathf.Abs(pos.y) * 2f,
                Color.Lerp(Color.yellow, Color.green, BlendValue)
                );
        }
    }
#endif
}
