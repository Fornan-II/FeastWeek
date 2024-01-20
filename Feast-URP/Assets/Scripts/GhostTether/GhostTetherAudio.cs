using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTetherAudio : AmbiencePlayer
{
    private const string k_Mixer_LPFMix = "AmbientLPFMix";

#pragma warning disable 0649
    [SerializeField] private GhostTether tether;
    [SerializeField] private AudioClip distantAmbienceSFX;
    [SerializeField] private AnimationCurve distanceMix;
    [SerializeField] private AnimationCurve dissolveResonanceFactor;

    private AudioCue _activeDistantAmbienceSFX;
    private float _tetherDissolveFactor = -1f;

    protected override IEnumerator Start()
    {
        _activeDistantAmbienceSFX = AudioManager.PlaySound(distantAmbienceSFX, transform, ambienceSFXSettings);

        tether.AddTetherDissolveCompleteListener(OnTetherDissolveComplete);
        tether.SetAmbientAudio(this);

        yield return base.Start();
    }

    private void Update()
    {
        if (!MainCamera.IsValid()) return;
        Chain.Node tetherNode = tether.GetNearestChainNode(MainCamera.RootTransform.position);
        transform.position = tetherNode.Position;
    }

    public override void OnOverlap()
    {
        //_lowPassFilter.cutoffFrequency = lowPassBlend.Evaluate(BlendFactor.Value);
        _activeAmbienceSFX.SetVolume(ambienceSFXSettings.Volume * volumeBlend.Evaluate(triggerVolume.BlendValue * BlendFactor.Value));
        _activeDistantAmbienceSFX.SetVolume(_activeAmbienceSFX.Settings.Volume);

        // k_MixerLPFMix expects a value -80 to 0
        AudioManager.Data.Mixer.SetFloat(k_Mixer_LPFMix, lowPassBlend.Evaluate(BlendFactor.Value));

        //base.OnOverlap();

        //if (_tetherDissolveFactor >= 0f)
        //{
        //    // LPF moved to mixer, this won't really work atm!!
        //
        //    // Tether is dissolving, audio has special behavior.
        //    _lowPassFilter.cutoffFrequency = _lowPassFilter.cutoffFrequency * _tetherDissolveFactor;
        //
        //    float value = dissolveResonanceFactor.Evaluate(_tetherDissolveFactor);
        //    _lowPassFilter.lowpassResonanceQ = value;
        //}

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
