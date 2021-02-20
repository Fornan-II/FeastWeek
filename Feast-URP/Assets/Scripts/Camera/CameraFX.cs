using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraFX
{
    // Properties
    [Header("Fade")]
    [SerializeField, ColorUsage(false, true)] private Color defaultFadeColor = Color.magenta;
    [Header("Impulse")]
    [SerializeField] private AnimationCurve impulseCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [Header("Screenshake")]
    [SerializeField] private float screenShakeExponent = 0.5f;
    [SerializeField, Range(0, 1)] private float screenShakeSmoothing = 0f;

    // Private members
    private MainCamera _mainCameraRef;
    private Coroutine _activeFadeRoutine;

    public void Init(MainCamera mainCameraInstance)
    {
        _mainCameraRef = mainCameraInstance;
        ResetFadeColorToDefault();
    }

    #region Crossfading
    public void SetFadeColor(Color color) => Shader.SetGlobalColor("_FadeColor", color);
    public void ResetFadeColorToDefault() => Shader.SetGlobalColor("_FadeColor", defaultFadeColor);

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

    #region Color Invert
    public void SetColorInvert(bool inverted)
    {
        Shader.SetGlobalFloat("_InvertValue", inverted ? 1 : 0);
    }
    #endregion

    #region Transform Effects
    private Coroutine StartTransformEffect(Func<Transform, IEnumerator> effect)
    {
        // Create new transform
        Transform effectTransform = new GameObject("~TransformEffect").transform;

        // Move effectTransform to MainCamera.Camera's parent
        // Then insert effectTransform between MainCamera.Camera and it's parent
        Util.MoveTransformToTarget(effectTransform, MainCamera.Camera.transform.parent, true);
        MainCamera.Camera.transform.SetParent(effectTransform);

        // Start transform effect
        return _mainCameraRef.StartCoroutine(effect.Invoke(effectTransform));
    }

    private void CleanUpTransformEffect(Transform effectTransform)
    {
        // Have all children of transform set their parent as effectTransform's parent
        for(int i = 0; i < effectTransform.childCount; ++i)
        {
            Util.MoveTransformToTarget(effectTransform.GetChild(i), effectTransform.parent, true);
        }

        // Delete effectTransform
        GameObject.Destroy(effectTransform.gameObject);
    }

    public void ApplyImpulse(Vector3 sourcePos, float strength, float duration = 0.5f)
    {
        IEnumerator ImpulseCoroutine(Transform t)
        {
            Vector3 startOffset = t.parent.InverseTransformVector(MainCamera.Camera.transform.position - sourcePos).normalized * strength;

            for(float timer = 0.0f; timer < duration; timer += Time.deltaTime)
            {
                t.localPosition = Vector3.Lerp(Vector3.zero, startOffset, impulseCurve.Evaluate(timer / duration));
                yield return null;
            }

            CleanUpTransformEffect(t);
        };

        StartTransformEffect(ImpulseCoroutine);
    }

    public void ApplyScreenShake(float strength, float duration = 0.5f)
    {
        IEnumerator ScreenShakeCoroutine(Transform t)
        {
            for (float timer = 0.0f; timer < duration; timer += Time.deltaTime)
            {
                Vector3 newPosition = UnityEngine.Random.insideUnitSphere * strength * Mathf.Clamp01(Mathf.Pow(1f - timer / duration, screenShakeExponent));
                t.localPosition = Vector3.Lerp(newPosition, t.localPosition, screenShakeSmoothing);
                yield return null;
            }

            CleanUpTransformEffect(t);
        };

        StartTransformEffect(ScreenShakeCoroutine);
    }

    public Action<float> ContinuousScreenShake(float initialStrength)
    {
        float strength = initialStrength;

        IEnumerator ScreenShakeCoroutine(Transform t)
        {
            while(strength > 0f)
            {
                Vector3 newPosition = UnityEngine.Random.insideUnitSphere * strength;
                t.localPosition = Vector3.Lerp(newPosition, t.localPosition, screenShakeSmoothing);
                yield return null;
            }

            CleanUpTransformEffect(t);
        };

        StartTransformEffect(ScreenShakeCoroutine);
        return (float value) => strength = value;
    }
    #endregion
}
