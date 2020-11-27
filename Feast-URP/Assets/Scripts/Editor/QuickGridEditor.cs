using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuickGrid))]
public class QuickGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Grid"))
        {
            (target as QuickGrid)?.GenerateGrid();
        }
        if (GUILayout.Button("Clear Grid"))
        {
            (target as QuickGrid)?.ClearGrid();
        }
    }
}
