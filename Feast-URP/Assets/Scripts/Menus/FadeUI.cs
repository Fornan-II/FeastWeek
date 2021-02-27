using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class FadeUI : MonoBehaviour
{
    public bool IsFading => _fadeRoutine != null;

    [SerializeField] private CanvasGroup uiGroup;
    [SerializeField] private float fadeDuration = 1f;

    private Coroutine _fadeRoutine;

    public void FadeIn(UnityAction OnFadeComplete = null)
    {
        float startAlpha = 0f;
        float duration = fadeDuration;
        if (_fadeRoutine != null)
        {
            PauseManager.Instance.StopCoroutine(_fadeRoutine);
            startAlpha = uiGroup.alpha;
            duration = fadeDuration * (1 - startAlpha);
        }
        _fadeRoutine = PauseManager.Instance.StartCoroutine(Fade(startAlpha, 1f, duration, OnFadeComplete));
    }

    public void FadeOut(UnityAction OnFadeComplete = null)
    {
        float startAlpha = 1f;
        float duration = fadeDuration;
        if (_fadeRoutine != null)
        {
            PauseManager.Instance.StopCoroutine(_fadeRoutine);
            startAlpha = uiGroup.alpha;
            duration = fadeDuration * startAlpha;
        }
        _fadeRoutine = PauseManager.Instance.StartCoroutine(Fade(startAlpha, 0f, duration, OnFadeComplete));
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha, float duration, UnityAction OnFadeComplete)
    {
        for (float timer  = 0.0f; timer < duration; timer += Time.unscaledDeltaTime)
        {
            uiGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }

        uiGroup.alpha = targetAlpha;

        _fadeRoutine = null;
        OnFadeComplete?.Invoke();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!uiGroup) uiGroup = GetComponent<CanvasGroup>();
    }
#endif
}
