using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiencePlayer : MonoBehaviour, ITriggerListener
{
    public ModifierSet BlendFactor = new ModifierSet(1f, ModifierSet.CalculateMode.MIN);

#pragma warning disable 0649
    [Header("Main Settings")]
    [SerializeReference] protected BaseTriggerVolume triggerVolume;
    [SerializeField] protected float fadeInTime = -1f;
    [SerializeField] protected AudioClip ambienceSFX;
    [SerializeField] protected AudioCue.CueSettings ambienceSFXSettings = AudioCue.CueSettings.Default;
    [SerializeField] protected AnimationCurve volumeBlend = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [Header("Low Pass Filter")]
    [SerializeField] protected AnimationCurve lowPassBlend = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    protected AudioCue _activeAmbienceSFX;
    protected AudioLowPassFilter _lowPassFilter;

    public virtual void OnOverlap()
    {
        _lowPassFilter.cutoffFrequency = lowPassBlend.Evaluate(triggerVolume.BlendValue * BlendFactor.Value);

        _activeAmbienceSFX.SetVolume(ambienceSFXSettings.Volume * volumeBlend.Evaluate(triggerVolume.BlendValue * BlendFactor.Value));
    }

    public virtual void OnOverlapStart() => _activeAmbienceSFX.Play();
    public virtual void OnOverlapExit() => _activeAmbienceSFX.Pause();

    protected virtual IEnumerator Start()
    {
        _activeAmbienceSFX = AudioManager.PlaySound(ambienceSFX, transform, ambienceSFXSettings);
        _lowPassFilter = _activeAmbienceSFX.gameObject.AddComponent<AudioLowPassFilter>();
        
        // Fade-in audio
        if (fadeInTime <= 0)
        {
            // OnOverlap has to be called at least once to init volume
            OnOverlap();
            yield break;
        }
        
        int instanceID = GetInstanceID();
        BlendFactor.SetModifier(instanceID, 0f);

        OnOverlap();

        for (float timer = 0.0f; timer < fadeInTime; timer += Time.deltaTime)
        {
            yield return null;
            BlendFactor.SetModifier(instanceID, timer / fadeInTime);
        }

        BlendFactor.RemoveModifier(instanceID);
    }

    protected virtual void OnEnable() => triggerVolume.AddListener(this);
    protected virtual void OnDisable() => triggerVolume.RemoveListener(this);
}
