using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip defaultFootstepSound;
    [SerializeField] private FootstepData footstepData;
    [SerializeField] private AudioCue.CueSettings footstepSoundSettings = AudioCue.CueSettings.Default;

    private Dictionary<FootstepSurface.SurfaceType, AudioClip> _footstepAudioDictionary;
    
    private void Awake() => _footstepAudioDictionary = footstepData.GetSurfaceTypeAudioClips();

    public void PlayFootstep(FootstepSurface.SurfaceType surfaceType)
    {
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
            AudioManager.PlaySound(defaultFootstepSound, transform.position, footstepSoundSettings);
        }
        else
        {
            AudioManager.PlaySound(_footstepAudioDictionary[surfaceType], transform.position, footstepSoundSettings);
        }
    }
}
