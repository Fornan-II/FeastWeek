using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceMultiplierSetter : CubeTriggerVolume
{
#pragma warning disable 0649
    [SerializeField] private AmbiencePlayer targetAmbience;
    [SerializeField] private Vector2 blendRemap = new Vector2(0f, 1f);

    private int _cachedInstanceID;

    private void Start()
    {
        _cachedInstanceID = GetInstanceID();
    }

    protected override void OnOverlap()
    {
        targetAmbience.BlendFactor.SetModifier(_cachedInstanceID, 1f - Mathf.Lerp(blendRemap.x, blendRemap.y, blendValue));
    }

    protected override void OnOverlapExit()
    {
        targetAmbience.BlendFactor.RemoveModifier(_cachedInstanceID);
    }
}
