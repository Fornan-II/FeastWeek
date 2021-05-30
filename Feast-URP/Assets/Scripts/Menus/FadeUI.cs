using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class FadeUI : MonoBehaviour
{
    public bool IsFading => _fadeRoutine != null;
    public float Alpha => uiGroup.alpha;

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
        uiGroup.blocksRaycasts = true;
        uiGroup.interactable = true;
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
        uiGroup.blocksRaycasts = false;
        uiGroup.interactable = false;
        _fadeRoutine = PauseManager.Instance.StartCoroutine(Fade(startAlpha, 0f, duration, OnFadeComplete));
    }

    public void SetVisible()
    {
        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);
        _fadeRoutine = null;
        uiGroup.blocksRaycasts = true;
        uiGroup.interactable = true;
        uiGroup.alpha = 1f;
    }

    public void SetClear()
    {
        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);
        _fadeRoutine = null;
        uiGroup.blocksRaycasts = false;
        uiGroup.interactable = false;
        uiGroup.alpha = 0f;
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
