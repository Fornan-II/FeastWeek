using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamEffectTester : MonoBehaviour
{
    public bool TestImpulse = true;
    public bool TestShake = true;
    public bool TestContinuousShake = true;

    [Header("Impulse settings")]
    [SerializeField] private float ImpulseStrength = 1f;
    [SerializeField] private Transform ImpulseSource;
    [SerializeField] private float ImpulseDuration = 0.5f;
    [Header("Screen Shake settings")]
    [SerializeField] private float ScreenShakeStrength = 1f;
    [SerializeField] private float ScreenShakeDuration = 0.5f;
    [Header("Continuous Screen Shake settings")]
    [SerializeField] private float ContinuousScreenShakeStrength = 0.5f;
    [SerializeField] private float ScreenShakeProximity = 2f;

    private bool _wasPressed;
    private System.Action<float> _continuousScreenShakeStrengthHandle;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.rKey.isPressed && !_wasPressed)
        {
            if (TestImpulse)
            {
                MainCamera.Effects.ApplyImpulse(ImpulseSource ? ImpulseSource.position : transform.position, ImpulseStrength, ImpulseDuration);
            }
            if (TestShake)
            {
                MainCamera.Effects.ApplyScreenShake(ScreenShakeStrength, ScreenShakeDuration);
            }
        }

        if (TestContinuousShake)
        {
            float distance = Vector3.Distance(MainCamera.RootTransform.position, transform.position);
            if (distance < ScreenShakeProximity)
            {
                float strength = ContinuousScreenShakeStrength * Mathf.InverseLerp(ScreenShakeProximity, 0, distance);

                if (_continuousScreenShakeStrengthHandle == null)
                    _continuousScreenShakeStrengthHandle = MainCamera.Effects.ContinuousScreenShake(strength);
                else
                    _continuousScreenShakeStrengthHandle.Invoke(strength);
            }
            else if (_continuousScreenShakeStrengthHandle != null)
            {
                _continuousScreenShakeStrengthHandle.Invoke(0f);
                _continuousScreenShakeStrengthHandle = null;
            }
        }

        _wasPressed = Keyboard.current.rKey.isPressed;
    }

    private void OnDisable()
    {
        if (_continuousScreenShakeStrengthHandle != null)
            _continuousScreenShakeStrengthHandle.Invoke(0f);
    }
}
