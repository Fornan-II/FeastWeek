using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// https://www.youtube.com/watch?v=FgWVW2PL1bQ
public class AnimatedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float randomOffset = 0.1f;

    private Vector3[] vertices;
    private Mesh mesh;

    // Update is called once per frame
    void Update()
    {
        text.ForceMeshUpdate();

        mesh = text.mesh;
        vertices = mesh.vertices;

        for(int i = 0; i < vertices.Length; ++i)
        {
            Vector3 offset = Random.insideUnitCircle * randomOffset;
            vertices[i] += offset;
        }

        mesh.vertices = vertices;
        text.canvasRenderer.SetMesh(mesh);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!text) text = GetComponent<TextMeshProUGUI>();
    }
#endif

}
