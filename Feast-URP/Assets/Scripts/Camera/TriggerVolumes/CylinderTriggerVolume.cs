using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderTriggerVolume : BaseTriggerVolume
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private float height = 1f;
    [SerializeField] private float innerRadius = 0.5f;
    [SerializeField] private float innerHeight = 0.5f;
    [SerializeField] private Transform testTransform;

    float testA;
    float testB;

    // Vertical Capped Cylinder; thanks Inigo!
    // https://iquilezles.org/articles/distfunctions/
    // Assumes local space
    private float sdCylinder(Vector3 camPos)
    {
        // Returns 0 at surface of cylinder, negative inside, positive outside. -1 at center.
        // Vector2 d = new Vector2(
        //     Util.GetXZPosition(camPos).magnitude - radius,
        //     Mathf.Abs(camPos.y) - height * 0.5f
        //     );
        // 
        // return Mathf.Min(Mathf.Max(d.x, d.y), 0f) + new Vector2(Mathf.Max(d.x, 0f) , Mathf.Max(d.y, 0f)).magnitude;

        float radiusBlend = Util.GetXZPosition(camPos).magnitude;
        radiusBlend = Mathf.InverseLerp(radius, innerRadius, radiusBlend);
        float heightBlend = Mathf.Abs(camPos.y);
        heightBlend = Mathf.InverseLerp(height * 0.5f, innerHeight * 0.5f, heightBlend);

        testA = radiusBlend;
        testB = heightBlend;

        return Mathf.Min(radiusBlend, heightBlend);
    }

    protected override bool IsMainCameraWithin()
    {
        Vector3 cameraPos = transform.InverseTransformPoint(MainCamera.RootTransform.position);
        float sdValue = sdCylinder(cameraPos);

        BlendValue = Mathf.Clamp01(-1f * sdValue);
        return sdValue <= 0;
    }

#if UNITY_EDITOR
    private void DrawCylinder(float r, float h, Color c)
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

    private void OnDrawGizmos()
    {
        DrawCylinder(radius, height, IsOverlapping ? Color.green : Color.yellow);
        DrawCylinder(innerRadius, innerHeight, IsOverlapping ? Color.green : Color.yellow);

        if (testTransform)
        {
            float value = sdCylinder(testTransform.localPosition);

            Color col = value < 0 ? Color.blue * Mathf.Abs(value) : Color.red * value;
            col.a = 1f;
            Util.DebugDrawPoint(testTransform.position, col);
            UnityEditor.Handles.Label(testTransform.position, string.Format("R: {0}\nH: {1}\nT: {2}", testA, testB, value));
        }
    }
#endif
}
