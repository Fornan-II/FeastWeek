using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class FadeUI : MonoBehaviour
{
    [SerializeField] private Image[] imageFields;
    [SerializeField] private TextMeshProUGUI[] textFields;
    [SerializeField] private float fadeDuration = 1f;

    private Coroutine _fadeRoutine;

    public void FadeIn(UnityAction OnFadeComplete = null)
    {
        if (_fadeRoutine != null) PauseManager.Instance.StopCoroutine(_fadeRoutine);
        _fadeRoutine = PauseManager.Instance.StartCoroutine(Fade(0, 1, OnFadeComplete));
    }

    public void FadeOut(UnityAction OnFadeComplete = null)
    {
        if (_fadeRoutine != null) PauseManager.Instance.StopCoroutine(_fadeRoutine);
        _fadeRoutine = PauseManager.Instance.StartCoroutine(Fade(1, 0, OnFadeComplete));
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha, UnityAction OnFadeComplete)
    {
        float fadeEndTime = Time.realtimeSinceStartup + fadeDuration;
        while (Time.realtimeSinceStartup < fadeEndTime)
        {
            float alpha = Mathf.Lerp(targetAlpha, startAlpha, (fadeEndTime - Time.realtimeSinceStartup) / fadeDuration);

            foreach (var img in imageFields)
            {
                Color col = img.color;
                col.a = alpha;
                img.color = col;
            }

            foreach (var txt in textFields)
            {
                Color col = txt.color;
                col.a = alpha;
                txt.color = col;
            }

            yield return null;

        }

        foreach (var img in imageFields)
        {
            Color col = img.color;
            col.a = targetAlpha;
            img.color = col;
        }

        foreach (var txt in textFields)
        {
            Color col = txt.color;
            col.a = targetAlpha;
            txt.color = col;
        }

        _fadeRoutine = null;
        OnFadeComplete?.Invoke();
    }

#if UNITY_EDITOR
    [ContextMenu("Get Fields in children")]
    private void GetFieldsInChildren()
    {
        imageFields = GetComponentsInChildren<Image>();
        textFields = GetComponentsInChildren<TextMeshProUGUI>();
    }
#endif
}
