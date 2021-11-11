using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private float defaultVolume = 0.85f;

    public List<AudioCue> ActiveCues => _activeSongCues;
    private List<AudioCue> _activeSongCues = new List<AudioCue>(1);
    private bool _mixingProhibited = false;
    private List<SongEvent> _songEventQueue = new List<SongEvent>(); // Using a list so that I can sort through them on insertion

#if UNITY_EDITOR // Useful for debugging
    [System.Serializable]
#endif
    private struct SongEvent
    {
        public float Time;
        public Action Event;
    }

    public void StopImmediately()
    {
        foreach(var cue in _activeSongCues)
        {
            cue.Stop();
        }
    }

    public AudioCue PlaySongDirectly(AudioClip song, bool looping = false)
    {
        AudioCue newSongCue = AudioManager.PlaySound(song, Vector3.zero, new AudioCue.CueSettings(AudioManager.MixerGroup.Music)
        {
            Volume = defaultVolume,
            Loop = looping
        });
        // Make cue have 2D audio
        newSongCue.Source.spatialBlend = 0f;

        // Make sure this song cue is set up to be tracked
        newSongCue.OnFinishedPlaying += () => _activeSongCues.Remove(newSongCue);
        _activeSongCues.Add(newSongCue);

        return newSongCue;
    }

    public void SetSongCueInactive(AudioCue cue)
    {
        if (_activeSongCues.Contains(cue))
        {
            _activeSongCues.Remove(cue);
        }
        else
        {
            Debug.LogWarning("[MusicManager] Tried to set song cue inactive that was not found in active song cues.");
        }

        cue.SetInactive();
    }

    public void MixSongs(AudioCue songPlayer1, AudioCue songPlayer2, float mix)
    {
        if (_mixingProhibited) return;

        // 0 is songPlayer1, 1 is songPlayer2
        songPlayer1.SetVolume(Mathf.Lerp(defaultVolume, 0, mix * mix));
        songPlayer2.SetVolume(Mathf.Lerp(0, defaultVolume, mix * mix));
    }

    public void FadeInNewSong(AudioClip song, float duration, bool looping = false) => StartCoroutine(FadeAudio(PlaySongDirectly(song, looping), 0f, defaultVolume, duration));

    public void FadeOutAnySongs(float duration)
    {
        foreach(var cue in _activeSongCues)
        {
            StartCoroutine(FadeAudio(cue, cue.Settings.Volume, 0f, duration));
        }
    }

    public void CrossfadeInNewSong(AudioClip song, float duration, bool looping = false)
    {
        FadeOutAnySongs(duration);
        FadeInNewSong(song, duration, looping);
    }

    private IEnumerator FadeAudio(AudioCue cue, float initial, float target, float duration)
    {
        // Prohibit mixing while fading audio as a hack to not have mixing and fading conflict
        _mixingProhibited = true;
        cue.SetVolume(initial);

        for (float timer = 0.0f; timer < duration; timer += Time.deltaTime)
        {
            yield return null;
            float t = timer / duration;
            cue.SetVolume(Mathf.Lerp(initial, target, t * t));
        }

        cue.SetVolume(target);
        if (cue.Settings.Volume <= 0)
            cue.Stop();
        _mixingProhibited = false;
    }

    public void AddSongEvent(float time, Action songEvent)
    {
        int i = 0;
        for(; i < _songEventQueue.Count; ++i)
        {
            if (_songEventQueue[i].Time < time)
                break;
        }
        _songEventQueue.Insert(i, new SongEvent() { Time = time, Event = songEvent });
    }

    private void Update()
    {
        if (_activeSongCues.Count > 0)
        {
            while(_songEventQueue.Count > 0 && _activeSongCues[0].Source.time >= _songEventQueue[0].Time)
            {
                _songEventQueue[0].Event();
                _songEventQueue.RemoveAt(0);
            }
        }

    }
}
