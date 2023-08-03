using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierSet
{
    public enum CalculateMode
    {
        MULTIPLY,
        SUM,
        MIN,
        MAX
    }

    public float BaseValue { get; protected set; }
    public CalculateMode Mode { get; protected set; }
    public float Value { get; protected set; }

    private Dictionary<int, float> _modifiers;

    public ModifierSet(float baseValue, CalculateMode mode)
    {
        BaseValue = baseValue;
        Mode = mode;
        Value = BaseValue;
        _modifiers = new Dictionary<int, float>();
    }

    public ModifierSet() : this(1f, CalculateMode.MULTIPLY) { }

    public void SetBaseValue(float newBaseValue)
    {
        if (BaseValue == newBaseValue) return;
        BaseValue = newBaseValue;
        RecalculateAll();
    }

    public void SetMode(CalculateMode newMode)
    {
        if (Mode == newMode) return;
        Mode = newMode;
        RecalculateAll();
    }

    public void SetModifier(int id, float modifierValue)
    {
        if (_modifiers.ContainsKey(id))
        {
            if(_modifiers[id] != modifierValue)
            {
                _modifiers[id] = modifierValue;
                RecalculateAll();
            }
        }
        else
        {
            _modifiers.Add(id, modifierValue);
            Value = CalculatePair(Value, modifierValue);
        }
    }

    public void RemoveModifier(int id)
    {
        if(_modifiers.ContainsKey(id))
        {
            _modifiers.Remove(id);
            RecalculateAll();
        }
    }

    private void RecalculateAll()
    {
        Value = BaseValue;
        foreach(float value in _modifiers.Values)
        {
            Value = CalculatePair(Value, value);
        }
    }

    private float CalculatePair(float a, float b)
    {
        switch (Mode)
        {
            case CalculateMode.MULTIPLY:
                return a * b;
            case CalculateMode.SUM:
                return a + b;
            case CalculateMode.MIN:
                return Mathf.Min(a, b);
            case CalculateMode.MAX:
                return Mathf.Max(a, b);
            default:
                throw new System.NotImplementedException();
        }
    }

    public void LogModifiers()
    {
        string msg = "Modifiers:\n{";
        foreach(var pair in _modifiers)
        {
            msg += string.Format("\n\t{0} : {1}", pair.Key, pair.Value);
        }
        Debug.Log(msg + "\n}");
    }
}
