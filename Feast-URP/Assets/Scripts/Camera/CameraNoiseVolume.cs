using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNoiseVolume : SphereTriggerVolume
{
    // Currently having multiple camera noise effects at once is not supported.
    // For now there are no instances in the game of this, so it's fine.

    [SerializeField] private float noisePulseStrength = 1;
    [SerializeField] private float noisePulseSpeed    = 1;
    [SerializeField] private float noisePulseExponent = 1;

    private float _startingNoisePulseStrength = 1;
    private float _startingNoisePulseSpeed    = 1;
    private float _startingNoisePulseExponent = 1;

    protected override void OnOverlapStart()
    {
        _startingNoisePulseStrength = MainCamera.Effects.NoisePulseStrength;
        _startingNoisePulseSpeed    = MainCamera.Effects.NoisePulseSpeed;
        _startingNoisePulseExponent = MainCamera.Effects.NoisePulseExponent;
    }

    protected override void OnOverlap()
    {
        MainCamera.Effects.SetCameraNoisePulseStrength ( Mathf.Lerp(_startingNoisePulseStrength, noisePulseStrength, blendValue ));
        MainCamera.Effects.SetCameraNoisePulseSpeed    ( Mathf.Lerp(_startingNoisePulseSpeed,    noisePulseSpeed,    blendValue ));
        MainCamera.Effects.SetCameraNoisePulseExponent ( Mathf.Lerp(_startingNoisePulseExponent, noisePulseExponent, blendValue ));
    }

    protected override void OnOverlapExit()
    {
        if (!MainCamera.IsValid()) return;

        MainCamera.Effects.SetCameraNoisePulseStrength ( _startingNoisePulseStrength );
        MainCamera.Effects.SetCameraNoisePulseSpeed    ( _startingNoisePulseSpeed    );
        MainCamera.Effects.SetCameraNoisePulseExponent ( _startingNoisePulseExponent );
    }
}
