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
    [Header("Camera Noise")]
    [SerializeField] private float nearBlobCameraNoise = 0.3f;
    private Song dangerMusicSong;

    private Action<float> _screenShakeEffect;

    private float BlendedScreenShakeStrength() => Mathf.Max(Mathf.Epsilon, screenShakeStrength * Mathf.Pow(blendValue, screenShakeBlendExponent));
    private float BlendedNoiseStrength()
    {
        Vector2 vecToCenter = Util.GetXZPosition(transform.position - MainCamera.Camera.transform.position).normalized;
        Vector2 lookDir = Util.GetXZPosition(MainCamera.Camera.transform.forward).normalized;
        float viewDot = Mathf.Clamp01(Vector2.Dot(
            vecToCenter,
            lookDir));
        return Mathf.Lerp(MainCamera.Effects.DefaultCameraNoise, nearBlobCameraNoise, blendValue * blendValue * viewDot);
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
        _screenShakeEffect = MainCamera.Effects.ContinuousScreenShake(BlendedScreenShakeStrength());

        dangerMusicSong = musicManagerRef.PlaySongDirectly(dangerMusic, true);
        dangerMusicSong.SongCue.SetVolume(0f);

        MainCamera.Effects.SetCameraNoise(BlendedNoiseStrength());
    }

    protected override void OnOverlap()
    {
        _screenShakeEffect(BlendedScreenShakeStrength());

        musicManagerRef.MixSongs(musicManagerRef.ActiveSongs[0], dangerMusicSong, dangerAudioCurve.Evaluate(blendValue));

        controllerRumbler.LowFrequencyRumble = lowFreqRumbleCurve.Evaluate(blendValue);
        controllerRumbler.HighFrequencyRumble = highFreqRumbleCurve.Evaluate(blendValue);

        MainCamera.Effects.SetCameraNoise(BlendedNoiseStrength());
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

        MainCamera.Effects.ResetCameraNoise();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!controllerRumbler) controllerRumbler = GetComponent<ControllerRumbler>();
    }

    private void OnDrawGizmosSelected()
    {
        if (!MainCamera.IsValid()) return;

        Vector3 camPos = MainCamera.Camera.transform.position;
        Vector2 vecToCenter = Util.GetXZPosition(transform.position - camPos).normalized;
        Vector2 lookDir = Util.GetXZPosition(MainCamera.Camera.transform.forward).normalized;
        float viewDot = Mathf.Clamp01(Vector2.Dot(
            vecToCenter,
            lookDir ));
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(camPos, Util.XZVector3(vecToCenter));
        Gizmos.color = Color.Lerp(Color.black, Color.blue, viewDot);
        Gizmos.DrawRay(camPos, Util.XZVector3(lookDir));
    }
#endif
}
