using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientAudioBoxTrigger : CubeTriggerVolume
{
#pragma warning disable 0649
    [SerializeField] private AmbiencePlayer ambiencePlayer;

    protected override void OnOverlapStart() => ambiencePlayer.StartAudio();
    protected override void OnOverlap() => ambiencePlayer.SetBlend(blendValue);
    protected override void OnOverlapExit() => ambiencePlayer.StopAudio();
}
