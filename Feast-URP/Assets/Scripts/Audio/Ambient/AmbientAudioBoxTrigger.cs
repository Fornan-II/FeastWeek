using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientAudioBoxTrigger : CubeTriggerVolume, ITriggerListener
{
#pragma warning disable 0649
    [SerializeField] private AmbiencePlayer ambiencePlayer;

    private void OnEnable() => AddListener(this);
    private void OnDisable() => RemoveListener(this);

    public void OnOverlapStart() { }// => ambiencePlayer.StartAudio();
    public void OnOverlap() { }// => ambiencePlayer.SetBlend(BlendValue);
    public void OnOverlapExit() { }// => ambiencePlayer.StopAudio();
}
