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
        UNKNOWN
    }

    [SerializeField] private SurfaceType type = SurfaceType.UNKNOWN;
    public SurfaceType Type => type;
}
