using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTetherAudio : AmbiencePlayer
{
#pragma warning disable 0649
    [SerializeField] private GhostTether tether;
    [SerializeField] private AudioClip distantAmbienceSFX;
    [SerializeField] private AnimationCurve distanceMix;
    [SerializeField] private AnimationCurve dissolveResonanceFactor;

    private AudioCue _activeDistantAmbienceSFX;
    private AudioLowPassFilter _distantLowPassFilter;
    private float _tetherDissolveFactor = -1f;

    protected override IEnumerator Start()
    {
        _activeDistantAmbienceSFX = AudioManager.PlaySound(distantAmbienceSFX, transform, ambienceSFXSettings);
        _distantLowPassFilter = _activeDistantAmbienceSFX.gameObject.AddComponent<AudioLowPassFilter>();

        tether.AddTetherDissolveCompleteListener(OnTetherDissolveComplete);
        tether.SetAmbientAudio(this);

        return base.Start();
    }

    private void Update()
    {
        if (!MainCamera.IsValid()) return;
        Chain.Node tetherNode = tether.GetNearestChainNode(MainCamera.RootTransform.position);
        transform.position = tetherNode.Position;
    }

    public override void OnOverlap()
    {
        base.OnOverlap();

        if (_tetherDissolveFactor >= 0f)
        {
            // Tether is dissolving, audio has special behavior.
            _lowPassFilter.cutoffFrequency = _lowPassFilter.cutoffFrequency * _tetherDissolveFactor;

            float value = dissolveResonanceFactor.Evaluate(_tetherDissolveFactor);
            _lowPassFilter.lowpassResonanceQ = value;
            _distantLowPassFilter.lowpassResonanceQ = value;
        }

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

    public void SetTetherDissolveAudio(float value) => _tetherDissolveFactor = value;
    private void OnTetherDissolveComplete() => gameObject.SetActive(false);
}
