using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Probably should implement all the same stuff as List or Array
[System.Serializable]
public class FloatField3D
{
    public Vector3Int FieldSize => fieldSize;

    [SerializeField, HideInInspector] private Vector3Int fieldSize = Vector3Int.zero;
    [SerializeField, HideInInspector] private float[] fieldItems;

    public FloatField3D()
    {
        fieldItems = new float[0];
    }

    public FloatField3D(int xSize, int ySize, int zSize)
    {
        fieldSize = new Vector3Int(xSize, ySize, zSize);
        fieldItems = new float[xSize * ySize * zSize];
    }

    public void SetValue(int x, int y, int z, float value)
    {
        if(TryGetIndex(x, y, z, out int index))
        {
            fieldItems[index] = value;
        }
    }

    public float GetValue(int x, int y, int z)
    {
        if (TryGetIndex(x, y, z, out int index))
        {
            return fieldItems[index];
        }
        return 0f;
    }

    public bool IsCoordValid(int x, int y, int z) => TryGetIndex(x, y, z, out _);

    private bool TryGetIndex(int x, int y, int z, out int index)
    {
        if(Util.IndexIsInRange(x, fieldSize.x) && Util.IndexIsInRange(y, fieldSize.y) && Util.IndexIsInRange(z, fieldSize.z))
        {
            index =
                x +                             // x
                fieldSize.x * y +               // y
                fieldSize.x * fieldSize.y * z;  // z

            return true;
        }

        index = -1;
        return false;
    }
}
