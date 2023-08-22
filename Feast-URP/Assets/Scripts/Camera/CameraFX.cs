﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraFX
{
    private class TransformEffect
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public float DefaultCameraNoise => cameraNoise.BaseValue;
    public float DefaultCameraNoisePulseStrength => noisePulseStrength.BaseValue;
    public float DefaultCameraNoisePulseSpeed => noisePulseSpeed.BaseValue;
    public float DefaultCameraNoisePulseExponent => noisePulseExponent.BaseValue;

    // Properties
    [Header("Fade")]
    [SerializeField, ColorUsage(false, true)] private Color defaultFadeColor = Color.magenta;
    [Header("Impulse")]
    [SerializeField] private AnimationCurve impulseCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [Header("Screenshake")]
    [SerializeField] private float screenShakeExponent = 0.5f;
    [SerializeField] private float defaultCameraShakeFrequency = 18f;
    [SerializeField] private float cameraShakeRotationMultiplier = 5f;
    [Header("Noise")]
    [SerializeField] private ModifierSet cameraNoise = new ModifierSet(0.003f, ModifierSet.CalculateMode.MAX);
    [SerializeField] private ModifierSet noisePulseStrength = new ModifierSet(0, ModifierSet.CalculateMode.MAX);
    [SerializeField] private ModifierSet noisePulseSpeed = new ModifierSet(1, ModifierSet.CalculateMode.MAX);
    [SerializeField] private ModifierSet noisePulseExponent = new ModifierSet(1, ModifierSet.CalculateMode.MAX);

    // Private members
    private MainCamera _mainCameraRef;
    private Coroutine _activeFadeRoutine;
    private List<TransformEffect> _transformEffects = new List<TransformEffect>();

    public void Init(MainCamera mainCameraInstance)
    {
        _mainCameraRef = mainCameraInstance;
        ResetFadeColorToDefault();
        Shader.SetGlobalFloat("_NoiseStrength", cameraNoise.Value);
    }

    public void Update(float deltaTime)
    {
        ApplyTransformEffects();
        ApplyCameraNoise();
    }

    #region Crossfading
    public void SetFadeColor(Color color) => Shader.SetGlobalColor("_FadeColor", color);
    public void ResetFadeColorToDefault() => Shader.SetGlobalColor("_FadeColor", defaultFadeColor);
    public void ManuallySetScreenFade(float value) => Shader.SetGlobalFloat("_ScreenFade", Mathf.Clamp01(value));

    public void CrossFade(float fadeDuration, bool fadeOut)
    {
        if (fadeDuration <= 0f)
            Shader.SetGlobalFloat("_ScreenFade", fadeOut ? 1f : 0f);
        else
        {
            if (_activeFadeRoutine != null) _mainCameraRef.StopCoroutine(_activeFadeRoutine);
            _activeFadeRoutine = _mainCameraRef.StartCoroutine(FadeScreen(fadeOut ? 0f : 1f, fadeOut ? 1f : 0f, fadeDuration));
        }
    }

    private IEnumerator FadeScreen(float from, float to, float duration)
    {
        for (float timer = 0.0f; timer < duration; timer += Time.deltaTime)
        {
            float tValue = Mathf.Lerp(from, to, timer / duration);
            Shader.SetGlobalFloat("_ScreenFade", tValue * tValue);
            yield return null;
        }
        Shader.SetGlobalFloat("_ScreenFade", to);
        _activeFadeRoutine = null;
    }
    #endregion
    
    public void SetColorInvert(bool inverted) => Shader.SetGlobalFloat("_InvertValue", inverted ? 1 : 0);

    public void ApplyCameraNoise(int sourceID, float value)
    {
        cameraNoise.SetModifier(sourceID, value);
        Shader.SetGlobalFloat("_NoiseStrength", cameraNoise.Value);
    }
    public void RemoveCameraNoise(int sourceID)
    {
        cameraNoise.RemoveModifier(sourceID);
        Shader.SetGlobalFloat("_NoiseStrength", cameraNoise.Value);
    }
    public void ResetCameraNoiseModifiers()
    {
        cameraNoise.ClearModifiers();
        Shader.SetGlobalFloat("_NoiseStrength", cameraNoise.Value);
    }


    public void ApplyCameraNoisePulse(int sourceID, float strength, float speed, float exponent)
    {
        noisePulseStrength.SetModifier(sourceID, strength);
        noisePulseSpeed.SetModifier(sourceID, speed);
        noisePulseExponent.SetModifier(sourceID, exponent);
    }

    public void RemoveCameraNoisePulse(int sourceID)
    {
        noisePulseStrength.RemoveModifier(sourceID);
        noisePulseSpeed.RemoveModifier(sourceID);
        noisePulseExponent.RemoveModifier(sourceID);
        Shader.SetGlobalFloat("_NoiseStrength", cameraNoise.Value);
    }

    #region Transform Effects
    public void ApplyTransformEffects()
    {
        _mainCameraRef.transform.localPosition = Vector3.zero;
        _mainCameraRef.transform.localRotation = Quaternion.identity;

        foreach(var effect in _transformEffects)
        {
            _mainCameraRef.transform.position += effect.position;
            _mainCameraRef.transform.localRotation *= effect.rotation;
        }
    }

    public void ApplyImpulse(Vector3 sourcePos, float strength, float duration = 0.5f)
    {
        IEnumerator ImpulseCoroutine()
        {
            Vector3 startOffset = (_mainCameraRef.transform.position - sourcePos).normalized * strength;
            TransformEffect t = new TransformEffect() { position = startOffset, rotation = Quaternion.identity };
            _transformEffects.Add(t);

            for(float timer = 0.0f; timer < duration; timer += Time.deltaTime)
            {
                t.position = Vector3.Lerp(Vector3.zero, startOffset, impulseCurve.Evaluate(timer / duration));
                yield return null;
            }

            _transformEffects.Remove(t);
        };

        _mainCameraRef.StartCoroutine(ImpulseCoroutine());
    }

    public void ApplyScreenShake(float strength, float duration = 0.5f) => ApplyScreenShake(strength, defaultCameraShakeFrequency, duration);

    public void ApplyScreenShake(float strength, float frequency, float duration)
    {
        IEnumerator ScreenShakeCoroutine()
        {
            TransformEffect t = new TransformEffect();
            _transformEffects.Add(t);

            for (float timer = 0.0f; timer < duration; timer += Time.deltaTime)
            {
                //UnityEngine.Random.insideUnitSphere * strength * Mathf.Clamp01(Mathf.Pow(1f - timer / duration, screenShakeExponent));
                float tValue = Mathf.Pow(1f - timer / duration, screenShakeExponent);
                t.position = new Vector3((Mathf.PerlinNoise(Time.timeSinceLevelLoad * frequency, 0f) * 2f - 1f) * strength * tValue * 0.5f,
                                        (Mathf.PerlinNoise(Time.timeSinceLevelLoad * frequency, 3f) * 2f - 1f) * strength * tValue * 0.5f,
                                        (Mathf.PerlinNoise(Time.timeSinceLevelLoad * frequency, 7f) * 2f - 1f) * strength * tValue * 0.5f);
                t.rotation = Quaternion.Euler(Vector3.forward * ((Mathf.PerlinNoise(Time.timeSinceLevelLoad * frequency, 11f) * 2f - 1f) * strength * 0.5f * tValue * cameraShakeRotationMultiplier));
                yield return null;
            }

            _transformEffects.Remove(t);
        };

        _mainCameraRef.StartCoroutine(ScreenShakeCoroutine());
    }

    public Action<float> ContinuousScreenShake(float initialStrength) => ContinuousScreenShake(initialStrength, defaultCameraShakeFrequency);

    // Oh wow that's uh... real "creative" of you there

    /// <summary>
    /// Apply a sceen shake to the camera as long as strength is above 0
    /// </summary>
    /// <param name="initialStrength"></param>
    /// <param name="frequency"></param>
    /// <returns>Action handle that can set screen shake strength on demand</returns>
    public Action<float> ContinuousScreenShake(float initialStrength, float frequency)
    {
        float strength = initialStrength;

        IEnumerator ScreenShakeCoroutine()
        {
            TransformEffect t = new TransformEffect();
            _transformEffects.Add(t);

            while(strength > 0f)
            {
                t.position = new Vector3((Mathf.PerlinNoise(Time.timeSinceLevelLoad * frequency, 0f) * 2f - 1f) * strength * 0.5f,
                                        (Mathf.PerlinNoise(Time.timeSinceLevelLoad * frequency, 3f) * 2f - 1f) * strength * 0.5f,
                                        (Mathf.PerlinNoise(Time.timeSinceLevelLoad * frequency, 7f) * 2f - 1f) * strength * 0.5f);
                t.rotation = Quaternion.Euler(Vector3.forward * ((Mathf.PerlinNoise(Time.timeSinceLevelLoad * frequency, 11f) * 2f - 1f) * strength * 0.5f * cameraShakeRotationMultiplier));
                yield return null;
            }

            _transformEffects.Remove(t);
        };

        _mainCameraRef.StartCoroutine(ScreenShakeCoroutine());
        return (float value) => strength = value;
    }
    #endregion

    private void ApplyCameraNoise()
    {
        if(noisePulseStrength.Value > 0 && noisePulseExponent.Value > 0f)
        {
            float value = noisePulseStrength.Value * Mathf.Pow(Mathf.PerlinNoise(Time.time * noisePulseSpeed.Value, 0f), noisePulseExponent.Value);

            Shader.SetGlobalFloat("_NoiseStrength", cameraNoise.Value + value);
        }
    }
}
