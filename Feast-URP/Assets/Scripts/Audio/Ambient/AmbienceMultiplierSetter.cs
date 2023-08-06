using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceMultiplierSetter : CubeTriggerVolume, ITriggerListener
{
#pragma warning disable 0649
    [SerializeField] private AmbiencePlayer targetAmbience;
    [SerializeField] private Vector2 blendRemap = new Vector2(0f, 1f);

    private int _cachedInstanceID;

    private void Start()
    {
        _cachedInstanceID = GetInstanceID();
    }

    private void OnEnable() => AddListener(this);
    private void OnDisable() => RemoveListener(this);

    public void OnOverlapStart() { }

    public void OnOverlap()
    {
        targetAmbience.BlendFactor.SetModifier(_cachedInstanceID, 1f - Mathf.Lerp(blendRemap.x, blendRemap.y, BlendValue));
    }

    public void OnOverlapExit()
    {
        targetAmbience.BlendFactor.RemoveModifier(_cachedInstanceID);
    }
}
