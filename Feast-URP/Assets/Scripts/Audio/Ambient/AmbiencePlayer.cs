using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiencePlayer : MonoBehaviour
{
    public ModifierSet BlendFactor = new ModifierSet(1f, ModifierSet.CalculateMode.MIN);

#pragma warning disable 0649
    [Header("Main Settings")]
    [SerializeField] protected float fadeInTime = -1f;
    [SerializeField] protected AudioClip ambienceSFX;
    [SerializeField] protected AudioCue.CueSettings ambienceSFXSettings = AudioCue.CueSettings.Default;
    [SerializeField] protected AnimationCurve volumeBlend = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [Header("Low Pass Filter")]
    [SerializeField] protected float cutOffFrequency;
    [SerializeField] protected AnimationCurve lowPassBlend = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    protected AudioCue _activeAmbienceSFX;
    protected AudioLowPassFilter _lowPassFilter;

    public virtual void SetBlend(float value)
    {
        _lowPassFilter.cutoffFrequency = cutOffFrequency * lowPassBlend.Evaluate(value * BlendFactor.Value);

        _activeAmbienceSFX.SetVolume(ambienceSFXSettings.Volume * volumeBlend.Evaluate(value * BlendFactor.Value));
    }

    public virtual void StartAudio() => _activeAmbienceSFX.Play();
    public virtual void StopAudio() => _activeAmbienceSFX.Stop(false);

    protected virtual IEnumerator Start()
    {
        _activeAmbienceSFX = AudioManager.PlaySound(ambienceSFX, transform, ambienceSFXSettings);
        _lowPassFilter = _activeAmbienceSFX.gameObject.AddComponent<AudioLowPassFilter>();

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
}
