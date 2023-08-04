using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlendOverrideCollection
{
#pragma warning disable 0649
    [Serializable]
    public struct BlendOverride
    {
        public CameraView From;
        public CameraView To;
        public ViewBlend Blend;
    }

    [SerializeField] private BlendOverride[] overrides;

    private Dictionary<CameraView, Dictionary<CameraView, ViewBlend>> _searchableOverrides;

    public void Initialize()
    {
        _searchableOverrides = new Dictionary<CameraView, Dictionary<CameraView, ViewBlend>>();
        foreach(BlendOverride item in overrides)
        {
            if(!_searchableOverrides.ContainsKey(item.From))
            {
                _searchableOverrides.Add(item.From, new Dictionary<CameraView, ViewBlend>());
            }

            if(_searchableOverrides[item.From].ContainsKey(item.To))
            {
                Debug.LogWarningFormat("Could not add duplicate override from {0} to {1}. Please ensure there are no duplicate overrides.", item.From, item.To);
            }
            else
            {
                _searchableOverrides[item.From][item.To] = item.Blend;
            }
        }
    }

    public bool TryGetBlendOverride(CameraView from, CameraView to, out ViewBlend blend)
    {
        blend = null;

        // Can't override null CameraViews
        if (!(from && to))
            return false;

        if(_searchableOverrides.TryGetValue(from, out var result))
        {
            if(result.TryGetValue(to, out blend))
            {
                return true;
            }
        }

        return false;
    }
}
