using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNoiseVolume : SphereTriggerVolume, ITriggerListener
{
    // Currently having multiple camera noise effects at once is not supported.
    // For now there are no instances in the game of this, so it's fine.

    [SerializeField] private float noisePulseStrength = 1;
    [SerializeField] private float noisePulseSpeed    = 1;
    [SerializeField] private float noisePulseExponent = 1;

    private float _startingNoisePulseStrength = 1;
    private float _startingNoisePulseSpeed    = 1;
    private float _startingNoisePulseExponent = 1;

    private void OnEnable()
    {
        AddListener(this);
    }

    private void OnDisable()
    {
        RemoveListener(this);
    }

    public void OnOverlapStart()
    {
        if (!MainCamera.IsValid()) return;

        _startingNoisePulseStrength = MainCamera.Effects.NoisePulseStrength;
        _startingNoisePulseSpeed    = MainCamera.Effects.NoisePulseSpeed;
        _startingNoisePulseExponent = MainCamera.Effects.NoisePulseExponent;
    }

    public void OnOverlap()
    {
        if (!MainCamera.IsValid()) return;

        MainCamera.Effects.SetCameraNoisePulseStrength ( Mathf.Lerp(_startingNoisePulseStrength, noisePulseStrength, BlendValue ));
        MainCamera.Effects.SetCameraNoisePulseSpeed    ( Mathf.Lerp(_startingNoisePulseSpeed,    noisePulseSpeed,    BlendValue ));
        MainCamera.Effects.SetCameraNoisePulseExponent ( Mathf.Lerp(_startingNoisePulseExponent, noisePulseExponent, BlendValue ));
    }

    public void OnOverlapExit()
    {
        if (!MainCamera.IsValid()) return;

        MainCamera.Effects.SetCameraNoisePulseStrength ( _startingNoisePulseStrength );
        MainCamera.Effects.SetCameraNoisePulseSpeed    ( _startingNoisePulseSpeed    );
        MainCamera.Effects.SetCameraNoisePulseExponent ( _startingNoisePulseExponent );
    }
}
