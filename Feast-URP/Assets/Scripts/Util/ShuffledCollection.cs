using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffledCollection<T>
{
    public int Size => _collection.Length;

    private T[] _collection;
    private int _nextIndex = 0;

    public ShuffledCollection()
    {
        _collection = new T[0];
    }

    public ShuffledCollection(T[] collection)
    {
        SetCollection(collection);
        Shuffle();
    }

    public T GetNext()
    {
        if (_nextIndex >= Size)
            Shuffle();
        return Size <= 0 ? default(T) : _collection[_nextIndex++];
    }

    public void Shuffle()
    {
        _nextIndex = 0;
        Util.ShuffleCollection(_collection);
    }

    public void SetCollection(T[] collection)
    {
        _collection = new T[collection.Length];
        collection.CopyTo(_collection, 0);
    }
}
