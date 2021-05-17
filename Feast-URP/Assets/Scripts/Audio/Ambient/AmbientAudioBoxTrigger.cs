using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientAudioBoxTrigger : CubeTriggerVolume
{
#pragma warning disable 0649
    [Header("Main Settings")]
    [SerializeField] private float fadeInTime = -1f;
    [SerializeField] private AudioSource source;
    [SerializeField] private float volume = 0.4f;
    [SerializeField] private AnimationCurve volumeBlend = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [Header("Low Pass Filter")]
    [SerializeField] private AudioLowPassFilter lowPassFilter;
    [SerializeField] private float cutOffFrequency;
    [SerializeField] private AnimationCurve lowPassBlend = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private IEnumerator Start()
    {
        if (fadeInTime <= 0) yield break;

        int instanceID = GetInstanceID();
        blendMultipliers.Add(instanceID, 0f);
        RecalculateBlendMultiplier();
        for(float timer = 0.0f; timer < fadeInTime; timer += Time.deltaTime)
        {
            yield return null;
            blendMultipliers[instanceID] = timer / fadeInTime;
            RecalculateBlendMultiplier();
        }
        blendMultipliers.Remove(instanceID);
        RecalculateBlendMultiplier();
    }

    protected override void OnOverlapStart()
    {
        source.volume = 0f;
        source.Play();
    }

    protected override void OnOverlap()
    {
        if(lowPassFilter)
        {
            lowPassFilter.cutoffFrequency = cutOffFrequency * lowPassBlend.Evaluate(blendValue * _blendMultiplier);
        }

        source.volume = volume * volumeBlend.Evaluate(blendValue * _blendMultiplier);
    }

    protected override void OnOverlapExit()
    {
        source.Stop();
    }

    // Modifier
    public Dictionary<int, float> blendMultipliers = new Dictionary<int, float>();
    private float _blendMultiplier = 1f;
    public void RecalculateBlendMultiplier()
    {
        _blendMultiplier = 1f;
        foreach(float value in blendMultipliers.Values)
        {
            _blendMultiplier = Mathf.Min(_blendMultiplier, value);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!source) source = GetComponent<AudioSource>();
    }
#endif
}
