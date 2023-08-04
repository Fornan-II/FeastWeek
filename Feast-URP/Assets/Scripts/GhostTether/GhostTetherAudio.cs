using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTetherAudio : AmbiencePlayer
{
#pragma warning disable 0649
    [SerializeField] private GhostTether tether;
    [SerializeField] private AudioClip distantAmbienceSFX;
    [SerializeField] private AnimationCurve distanceBlend;
    [SerializeField] private AnimationCurve distanceMix;

    private AudioCue _activeDistantAmbienceSFX;
    private AudioLowPassFilter _distantLowPassFilter;
    private float _cachedDistanceToCamera;
    private bool _audioActive = false;

    protected override IEnumerator Start()
    {
        _activeAmbienceSFX = AudioManager.PlaySound(distantAmbienceSFX, transform, ambienceSFXSettings);
        _distantLowPassFilter = _activeAmbienceSFX.gameObject.AddComponent<AudioLowPassFilter>();

        return base.Start();
    }

    private void Update()
    {
        // Make work for an arbitrary number of tethers, as opposed to one.
        // There are two tethers!
        // Replace basic AudioSource with AudioCue

        if(!MainCamera.IsValid())
        {
            if(_audioActive)
            {
                StopAudio();
                _audioActive = false;
            }

            return;
        }

        Chain.Node tetherNode = tether.GetNearestChainNode(MainCamera.RootTransform.position);
        transform.position = tetherNode.Position;

        _cachedDistanceToCamera = transform.InverseTransformPoint(MainCamera.RootTransform.position).magnitude;

        if(_cachedDistanceToCamera <= Util.AnimationCurveLengthTime(distanceBlend))
        {
            SetBlend(distanceBlend.Evaluate(_cachedDistanceToCamera));

            if(!_audioActive)
            {
                StartAudio();
                _audioActive = true;
            }
        }
        else if(_audioActive)
        {
            StopAudio();
            _audioActive = false;
        }
    }

    public override void SetBlend(float value)
    {
        base.SetBlend(value);

        _distantLowPassFilter.cutoffFrequency = _lowPassFilter.cutoffFrequency;
        AudioCueEffects.Mix(_activeAmbienceSFX, _activeDistantAmbienceSFX, distanceMix.Evaluate(_cachedDistanceToCamera));
    }

    public override void StartAudio()
    {
        base.StartAudio();
        _activeDistantAmbienceSFX.Play();
    }

    public override void StopAudio()
    {
        base.StopAudio();
        _activeDistantAmbienceSFX.Stop(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _audioActive ? Color.green : Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, Util.AnimationCurveLengthTime(distanceBlend));
    }
#endif
}
