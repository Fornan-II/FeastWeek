using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Probably should implement all the same stuff as List or Array

[System.Serializable]
public class GenericField3D<T>
{
    public Vector3Int FieldSize => fieldSize;

    [SerializeField, HideInInspector] protected Vector3Int fieldSize = Vector3Int.zero;
    [SerializeField] protected T[] fieldItems;

    public GenericField3D()
    {
        fieldItems = new T[0];
    }

    public GenericField3D(int xSize, int ySize, int zSize)
    {
        fieldSize = new Vector3Int(xSize, ySize, zSize);
        fieldItems = new T[xSize * ySize * zSize];
    }

    public void SetValue(int x, int y, int z, T value)
    {
        if(TryGetIndex(x, y, z, out int index))
        {
            fieldItems[index] = value;
        }
    }

    public T GetValue(int x, int y, int z)
    {
        if (TryGetIndex(x, y, z, out int index))
        {
            return fieldItems[index];
        }
        return default(T);
    }

    public bool IsCoordValid(int x, int y, int z) => TryGetIndex(x, y, z, out _);

    protected bool TryGetIndex(int x, int y, int z, out int index)
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
