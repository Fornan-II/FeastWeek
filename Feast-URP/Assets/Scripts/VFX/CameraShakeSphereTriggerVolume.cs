using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeSphereTriggerVolume : SphereTriggerVolume
{
    [SerializeField] private float screenShakeStrength = 0.25f;
    [SerializeField] private float screenShakeBlendExponent = 1f;

    private Action<float> _screenShakeEffect;

    private float BlendedStrength() => Mathf.Max(Mathf.Epsilon, screenShakeStrength * Mathf.Pow(blendValue, screenShakeBlendExponent));

    private void OnDisable()
    {
        if (_screenShakeEffect != null)
        {
            _screenShakeEffect(0f);
            _screenShakeEffect = null;
        }
    }

    protected override void OnOverlapStart()
    {
        _screenShakeEffect = MainCamera.Effects.ContinuousScreenShake(BlendedStrength());
    }

    protected override void OnOverlap()
    {
        _screenShakeEffect(BlendedStrength());
    }

    protected override void OnOverlapExit()
    {
        _screenShakeEffect(0f);
        _screenShakeEffect = null;
    }
}
