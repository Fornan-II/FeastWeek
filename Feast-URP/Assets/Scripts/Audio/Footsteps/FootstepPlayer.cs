using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepPlayer : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private AudioClip defaultFootstepSound;
    [SerializeField] private FootstepData footstepData;
    [SerializeField] private AudioCue.CueSettings footstepSoundSettings = AudioCue.CueSettings.Default;

    private Dictionary<FootstepSurface.SurfaceType, AudioClip[]> _footstepAudioDictionary;
    private FootstepSurface.SurfaceType _previousSurfaceType = FootstepSurface.SurfaceType.UNKNOWN;
    private int _previousAudioClipIndex = -1;
    
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
            int audioClipIndex = 0;
            if(_footstepAudioDictionary[surfaceType].Length > 1)
            {
                // Don't repeat thes same sound twice if there are multiple sound options
                do
                {
                    audioClipIndex = Random.Range(0, _footstepAudioDictionary[surfaceType].Length);
                } while (surfaceType == _previousSurfaceType && audioClipIndex == _previousAudioClipIndex);
            }
            _previousSurfaceType = surfaceType;
            _previousAudioClipIndex = audioClipIndex;

            AudioManager.PlaySound(
                _footstepAudioDictionary[surfaceType][audioClipIndex],
                transform.position,
                footstepSoundSettings
                );
        }
    }
}
