using System.Collections.Generic;
using UnityEngine;

public class IndexShuffler
{
    public int Size => _indexStack.Length;

    private int[] _indexStack;
    private int _nextIndex = 0;

    public IndexShuffler()
    {
        _indexStack = new int[0];
    }
    public IndexShuffler(int size)
    {
        _indexStack = new int[size];
        InitIndexStack();
    }

    public int GetNextIndex()
    {
        if (_nextIndex >= _indexStack.Length)
            Shuffle();

        return Size <= 0 ? -1 : _indexStack[_nextIndex++];
    }

    public void SetSize(int size)
    {
        _indexStack = new int[size];
        InitIndexStack();
    }

    private void InitIndexStack()
    {
        for (int i = 0; i < Size; ++i)
        {
            _indexStack[i] = i;
        }
    }

    private void Shuffle()
    {
        _nextIndex = 0;
        Util.ShuffleCollection(_indexStack);
    }
}
