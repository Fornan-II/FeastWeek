using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloatField3D : GenericField3D<float>
{
    public FloatField3D() : base() { }
    public FloatField3D(int xSize, int ySize, int zSize) : base(xSize, ySize, zSize) { }
}
