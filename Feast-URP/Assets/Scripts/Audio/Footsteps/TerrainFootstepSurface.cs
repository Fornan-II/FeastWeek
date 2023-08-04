using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFootstepSurface : FootstepSurface
{
#pragma warning disable 0649
    [SerializeField] private Mesh terrainMesh;

    // https://docs.unity3d.com/ScriptReference/RaycastHit-triangleIndex.html
    public override SurfaceType GetSurfaceType(RaycastHit hitInfo)
    {
        if( hitInfo.triangleIndex * 3 + 2 >= terrainMesh.triangles.Length
            || hitInfo.triangleIndex < 0)
        {
            return _surfaceType;
        }

        Color c0 = terrainMesh.colors[terrainMesh.triangles[hitInfo.triangleIndex * 3 + 0]];
        Color c1 = terrainMesh.colors[terrainMesh.triangles[hitInfo.triangleIndex * 3 + 1]];
        Color c2 = terrainMesh.colors[terrainMesh.triangles[hitInfo.triangleIndex * 3 + 2]];

        // I could get the exact world coordinate by accessing the vertex positions and doing world transformations...
        // which could then be used with hitInfo to get the exact point to use for interpolation for the color.
        // Or, I could just assume the center of the current triangle for simplification and go with that.
        // Or I could get the color of the nearest vertex instead, and just sample that?
        return _surfaceType;
    }
}
