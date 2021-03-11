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
    private FootstepSurface.SurfaceType previousSurfaceType = FootstepSurface.SurfaceType.UNKNOWN;
    private IndexShuffler _indexShuffler;
    
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
            int audioClipIndex = Random.Range(0, _footstepAudioDictionary[surfaceType].Length);
            // use shufflers, one per surface type

            AudioManager.PlaySound(
                _footstepAudioDictionary[surfaceType][audioClipIndex],
                transform.position,
                footstepSoundSettings
                );
        }
    }
}
