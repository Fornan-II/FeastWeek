using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceMultiplierSetter : CubeTriggerVolume
{
#pragma warning disable 0649
    [SerializeField] private AmbientAudioBoxTrigger targetAmbience;
    [SerializeField] private Vector2 blendRemap = new Vector2(0f, 1f);

    private int _cachedInstanceID;

    private void Start()
    {
        _cachedInstanceID = GetInstanceID();
    }

    protected override void OnOverlap()
    {
        if(targetAmbience.blendMultipliers.ContainsKey(_cachedInstanceID))
        {
            targetAmbience.blendMultipliers[_cachedInstanceID] = 1f - Mathf.Lerp(blendRemap.x, blendRemap.y, blendValue);
        }
        else
        {
            targetAmbience.blendMultipliers.Add(_cachedInstanceID, 1f - Mathf.Lerp(blendRemap.x, blendRemap.y, blendValue));
        }
        targetAmbience.RecalculateBlendMultiplier();
    }

    protected override void OnOverlapExit()
    {
        targetAmbience.blendMultipliers.Remove(_cachedInstanceID);
        targetAmbience.RecalculateBlendMultiplier();
    }
}
