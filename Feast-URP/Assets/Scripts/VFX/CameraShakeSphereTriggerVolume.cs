using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeSphereTriggerVolume : SphereTriggerVolume
{
    [SerializeField] private float screenShakeStrength = 0.25f;
    [SerializeField] private float screenShakeBlendExponent = 1f;

    // I'm adding this here because I want the audio to blend based on proximity, just like the camera shake
    // This class should be renamed to be more specific to the death blob, but it's a bit late for that now.
    [SerializeField] private AudioClip dangerMusic;
    [SerializeField] private AnimationCurve dangerAudioCurve;

    private MusicManager musicManagerRef;
    private AudioCue dangerMusicCue;

    private Action<float> _screenShakeEffect;

    private float BlendedStrength() => Mathf.Max(Mathf.Epsilon, screenShakeStrength * Mathf.Pow(blendValue, screenShakeBlendExponent));

    private void Start()
    {
        // Ewwww
        musicManagerRef = FindObjectOfType<MusicManager>();
    }

    private void OnDisable()
    {
        if (_screenShakeEffect != null)
        {
            _screenShakeEffect(0f);
            _screenShakeEffect = null;
        }
    }

    protected override void OnOverlapStart()
    {
        _screenShakeEffect = MainCamera.Effects.ContinuousScreenShake(BlendedStrength());
        dangerMusicCue = musicManagerRef.PlaySongDirectly(dangerMusic, true);
        dangerMusicCue.SetVolume(0f);
    }

    protected override void OnOverlap()
    {
        _screenShakeEffect(BlendedStrength());
        musicManagerRef.MixSongs(musicManagerRef.ActiveCues[0], dangerMusicCue, dangerAudioCurve.Evaluate(blendValue));
    }

    protected override void OnOverlapExit()
    {
        _screenShakeEffect(0f);
        _screenShakeEffect = null;
        musicManagerRef.MixSongs(musicManagerRef.ActiveCues[0], dangerMusicCue, 0);
        musicManagerRef.SetSongCueInactive(dangerMusicCue);
        dangerMusicCue = null;
    }
}
