using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNoiseVolume : SphereTriggerVolume, ITriggerListener
{
    [SerializeField] private float noisePulseStrength = 1;
    [SerializeField] private float noisePulseSpeed    = 1;
    [SerializeField] private float noisePulseExponent = 1;

    private void OnEnable()
    {
        AddListener(this);
    }

    private void OnDisable()
    {
        RemoveListener(this);
    }

    public void OnOverlapStart() { }

    public void OnOverlap()
    {
        if (!MainCamera.IsValid()) return;

        MainCamera.Effects.ApplyCameraNoisePulse(
            GetInstanceID(),
            Mathf.Lerp(0, noisePulseStrength, BlendValue),
            Mathf.Lerp(0, noisePulseSpeed, BlendValue),
            Mathf.Lerp(0, noisePulseExponent, BlendValue)
            );
    }

    public void OnOverlapExit()
    {
        if (!MainCamera.IsValid()) return;

        MainCamera.Effects.RemoveCameraNoisePulse(GetInstanceID());
    }
}
