using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSurface : MonoBehaviour
{
    public enum SurfaceType
    {
        DIRT,
        STONE,
        WOOD,
        GLASS,
        UNKNOWN
    }

    [SerializeField] protected SurfaceType _surfaceType = SurfaceType.UNKNOWN;
    public virtual SurfaceType GetSurfaceType() => _surfaceType;
    public virtual SurfaceType GetSurfaceType(RaycastHit hitInfo) => GetSurfaceType();
}
