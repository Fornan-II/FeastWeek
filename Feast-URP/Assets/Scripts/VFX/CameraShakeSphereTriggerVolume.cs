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
    [Header("Danger Music")]
    [SerializeField] private MusicManager musicManagerRef;
    [SerializeField] private AudioClip dangerMusic;
    [SerializeField] private AnimationCurve dangerAudioCurve;
    [Header("Controller rumble")]
    [SerializeField] private ControllerRumbler controllerRumbler;
    [SerializeField] private AnimationCurve lowFreqRumbleCurve;
    [SerializeField] private AnimationCurve highFreqRumbleCurve;
    private Song dangerMusicSong;

    private Action<float> _screenShakeEffect;

    private float BlendedStrength() => Mathf.Max(Mathf.Epsilon, screenShakeStrength * Mathf.Pow(blendValue, screenShakeBlendExponent));

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
        dangerMusicSong = musicManagerRef.PlaySongDirectly(dangerMusic, true);
        dangerMusicSong.SongCue.SetVolume(0f);
    }

    protected override void OnOverlap()
    {
        _screenShakeEffect(BlendedStrength());

        musicManagerRef.MixSongs(musicManagerRef.ActiveSongs[0], dangerMusicSong, dangerAudioCurve.Evaluate(blendValue));

        controllerRumbler.LowFrequencyRumble = lowFreqRumbleCurve.Evaluate(blendValue);
        controllerRumbler.HighFrequencyRumble = highFreqRumbleCurve.Evaluate(blendValue);
    }

    protected override void OnOverlapExit()
    {
        _screenShakeEffect(0f);
        _screenShakeEffect = null;

        musicManagerRef.MixSongs(musicManagerRef.ActiveSongs[0], dangerMusicSong, 0);
        musicManagerRef.SetSongCueInactive(dangerMusicSong);
        dangerMusicSong = null;

        controllerRumbler.LowFrequencyRumble = 0f;
        controllerRumbler.HighFrequencyRumble = 0f;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!controllerRumbler) controllerRumbler = GetComponent<ControllerRumbler>();
    }
#endif
}
