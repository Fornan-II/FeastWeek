using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAudioPlayer : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioCue.CueSettings clipSettings = AudioCue.CueSettings.Default;
    [SerializeField] private bool playOnAwake = false;

    private AudioCue _activeCue;

    private void Start()
    {
        if(playOnAwake)
        {
            Play();
        }
    }

    public void Play()
    {
        Stop();
        _activeCue = AudioManager.PlaySound(audioClip, transform, clipSettings);
    }

    public void Stop()
    {
        if (_activeCue)
        {
            _activeCue.Stop();
            _activeCue = null;
        }
    }

    public void FadeIn(float duration)
    {
        Play();
        _activeCue.FadeIn(clipSettings.Volume, duration);
    }

    public void FadeOut(float duration)
    {
        if(_activeCue)
        {
            _activeCue.FadeOut(duration, () => _activeCue = null);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, clipSettings.MaxDistance);
    }
#endif

}
