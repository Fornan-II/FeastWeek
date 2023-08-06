using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTetherAudio : AmbiencePlayer
{
#pragma warning disable 0649
    [SerializeField] private GhostTether tether;
    [SerializeField] private AudioClip distantAmbienceSFX;
    [SerializeField] private AnimationCurve distanceMix;

    private AudioCue _activeDistantAmbienceSFX;
    private AudioLowPassFilter _distantLowPassFilter;
    private bool _audioActive = false;

    protected override IEnumerator Start()
    {
        _activeDistantAmbienceSFX = AudioManager.PlaySound(distantAmbienceSFX, transform, ambienceSFXSettings);
        _distantLowPassFilter = _activeDistantAmbienceSFX.gameObject.AddComponent<AudioLowPassFilter>();

        return base.Start();
    }

    private void Update()
    {
        // Make work for an arbitrary number of tethers, as opposed to one.
        // There are two tethers!

        if (!MainCamera.IsValid()) return;

        Chain.Node tetherNode = tether.GetNearestChainNode(MainCamera.RootTransform.position);
        transform.position = tetherNode.Position;
    }

    public override void OnOverlap()
    {
        base.OnOverlap();

        _activeDistantAmbienceSFX.SetVolume(_activeAmbienceSFX.Settings.Volume);
        _distantLowPassFilter.cutoffFrequency = _lowPassFilter.cutoffFrequency;

        AudioCueEffects.Mix(_activeDistantAmbienceSFX, _activeAmbienceSFX, distanceMix.Evaluate(triggerVolume.BlendValue));
    }

    public override void OnOverlapStart()
    {
        base.OnOverlapStart();
        _activeDistantAmbienceSFX.Play();
    }

    public override void OnOverlapExit()
    {
        base.OnOverlapExit();
        _activeDistantAmbienceSFX.Pause();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _audioActive ? Color.green : Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
    }
#endif
}
