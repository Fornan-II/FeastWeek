using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceTest : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [ContextMenu("Play Source")]
    private void PlaySource() => audioSource.Play();

    [ContextMenu("Stop Source")]
    private void StopSource() => audioSource.Stop();

    [ContextMenu("Pause Source")]
    private void PauseSource() => audioSource.Pause();

    [ContextMenu("Log isPlaying")]
    private void GetIsPlaying() => Debug.Log(audioSource.isPlaying);

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
    }
#endif
}
