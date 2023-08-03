using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiencePlayer : MonoBehaviour
{
#pragma warning disable 0649
    [Header("Main Settings")]
    [SerializeField] protected float fadeInTime = -1f;
    [SerializeField] protected AudioSource source;
    [SerializeField] protected float volume = 0.4f;
    [SerializeField] protected AnimationCurve volumeBlend = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [Header("Low Pass Filter")]
    [SerializeField] protected AudioLowPassFilter lowPassFilter;
    [SerializeField] protected float cutOffFrequency;
    [SerializeField] protected AnimationCurve lowPassBlend = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public ModifierSet BlendFactor = new ModifierSet(1f, ModifierSet.CalculateMode.MIN);

    public virtual void SetBlend(float value)
    {
        if (lowPassFilter)
        {
            lowPassFilter.cutoffFrequency = cutOffFrequency * lowPassBlend.Evaluate(value * BlendFactor.Value);
        }

        source.volume = volume * volumeBlend.Evaluate(value * BlendFactor.Value);
    }

    public virtual void StartAudio() => source.Play();
    public virtual void StopAudio() => source.Stop();

    protected IEnumerator Start()
    {
        SetBlend(0f);

        // Fade-in audio
        if (fadeInTime <= 0) yield break;

        int instanceID = GetInstanceID();
        BlendFactor.SetModifier(instanceID, 0f);

        for (float timer = 0.0f; timer < fadeInTime; timer += Time.deltaTime)
        {
            yield return null;
            BlendFactor.SetModifier(instanceID, timer / fadeInTime);
        }

        BlendFactor.RemoveModifier(instanceID);
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (!source) source = GetComponent<AudioSource>();
        if (!lowPassFilter) lowPassFilter = GetComponent<AudioLowPassFilter>();
    }
#endif
}
