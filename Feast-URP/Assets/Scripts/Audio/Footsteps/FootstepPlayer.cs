using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepPlayer : MonoBehaviour
{
    public AudioCue.CueSettings FootStepSoundSettings => footstepSoundSettings;

    public bool Mute = false;

#pragma warning disable 0649
    [SerializeField] private AudioClip defaultFootstepSound;
    [SerializeField] private FootstepData footstepData;
    [SerializeField] private AudioCue.CueSettings footstepSoundSettings = AudioCue.CueSettings.Default;

    private Dictionary<FootstepSurface.SurfaceType, ShuffledCollection<AudioClip>> _footstepAudioDictionary;

    private void Awake()
    {
        // Using a ShuffledCollection instead of raw dictionary to minimize number of repeated footstep sounds
        _footstepAudioDictionary = new Dictionary<FootstepSurface.SurfaceType, ShuffledCollection<AudioClip>>();
        foreach(var item in footstepData.GetSurfaceTypeAudioClips())
        {
            _footstepAudioDictionary.Add(item.Key, new ShuffledCollection<AudioClip>(item.Value));
        }
    }

    public void SetMute(bool value) => Mute = value;

    public void PlayFootstep(FootstepSurface.SurfaceType surfaceType) => PlayFootstep(surfaceType, footstepSoundSettings);

    public void PlayFootstep(FootstepSurface.SurfaceType surfaceType, AudioCue.CueSettings soundSettings)
    {
        if (Mute) return;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        bool unsupportedSound = !(surfaceType == FootstepSurface.SurfaceType.UNKNOWN || _footstepAudioDictionary.ContainsKey(surfaceType));

        if (surfaceType == FootstepSurface.SurfaceType.UNKNOWN || unsupportedSound)
        {
            if (unsupportedSound)
                Debug.LogWarningFormat("Attempting to play unsupported footstep sound {0}!", surfaceType.ToString());
#else
        if (surfaceType == FootstepSurface.SurfaceType.UNKNOWN || !_footstepAudioDictionary.ContainsKey(surfaceType))
        {
#endif
            AudioManager.PlaySound(defaultFootstepSound, transform.position, soundSettings);
        }
        else
        {
            AudioManager.PlaySound(
                _footstepAudioDictionary[surfaceType].GetNext(),
                transform.position,
                soundSettings
                );
        }
    }
}
