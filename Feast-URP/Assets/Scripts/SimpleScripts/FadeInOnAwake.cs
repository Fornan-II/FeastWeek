using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOnAwake : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private float fadeInTime = 3f;

    private float _volume;

    private void Awake()
    {
        if (!source) return;

        _volume = source.volume;
        source.volume = 0f;
    }

    private IEnumerator Start()
    {
        for(float timer = 0.0f; timer < fadeInTime; timer += Time.deltaTime)
        {
            yield return null;
            float t = timer / fadeInTime;
            source.volume = Mathf.Lerp(0f, _volume, t * t);
        }

        source.volume = _volume;
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        for (float timer = 0.0f; timer < duration; timer += Time.deltaTime)
        {
            yield return null;
            float t = timer / duration;
            source.volume = Mathf.Lerp(_volume, 0f, t * t);
        }

        source.volume = 0f;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!source) source = GetComponent<AudioSource>();
    }
#endif
}
