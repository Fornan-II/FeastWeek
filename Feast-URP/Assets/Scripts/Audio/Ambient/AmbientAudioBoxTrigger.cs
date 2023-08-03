using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientAudioBoxTrigger : CubeTriggerVolume
{
    [SerializeField] private AmbiencePlayer ambiencePlayer;

    protected override void OnOverlapStart() => ambiencePlayer.StartAudio();
    protected override void OnOverlap() => ambiencePlayer.SetBlend(blendValue);
    protected override void OnOverlapExit() => ambiencePlayer.StopAudio();
}
