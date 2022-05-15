using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private float defaultVolume = 0.85f;

    public List<Song> ActiveSongs { get; private set; } = new List<Song>();

    public void StopImmediately()
    {
        foreach(var song in ActiveSongs)
        {
            song.SongCue.Stop();
        }
    }

    public Song PlaySongDirectly(AudioClip song, bool looping = false)
    {
        Song newSong = new Song(AudioManager.PlaySound(song, Vector3.zero, new AudioCue.CueSettings(AudioManager.MixerGroup.Music)
        {
            Volume = defaultVolume,
            Loop = looping,
            Is3D = false
        }));

        // Make sure this song cue is set up to be tracked
        newSong.SongCue.OnFinishedPlaying += () => SetSongCueInactive(newSong);
        ActiveSongs.Add(newSong);

        return newSong;
    }

    public void SetSongCueInactive(Song song)
    {
        if (ActiveSongs.Contains(song))
        {
            ActiveSongs.Remove(song);
        }
        else
        {
            Debug.LogWarning("[MusicManager] Tried to set song cue inactive that was not found in active song cues.");
        }

        song.SongCue.SetInactive();
    }

    public void MixSongs(Song song1, Song song2, float mix)
    {
        // Don't mix if the songs are fading, otherwise SetVolume would compete and sound weird
        if (song1.SongCue.IsFading || song2.SongCue.IsFading)
            return;

        // 0 is songPlayer1, 1 is songPlayer2
        song1.SongCue.SetVolume(Mathf.Lerp(defaultVolume, 0, mix * mix));
        song2.SongCue.SetVolume(Mathf.Lerp(0, defaultVolume, mix * mix));
    }

    public Song FadeInNewSong(AudioClip song, float duration, bool looping = false)
    {
        Song newSong = PlaySongDirectly(song, looping);
        newSong.SongCue.FadeIn(newSong.SongCue.Settings.Volume, duration);
        return newSong;
    }

    public void FadeOutAnySongs(float duration)
    {
        for(int i = 0; i < ActiveSongs.Count; ++i)
        {
            // Doing things this way because we want the song at i right now,
            // not the song at i when the fade is done.
            Song thisSong = ActiveSongs[i];
            ActiveSongs[i].SongCue.FadeOut(duration, () => SetSongCueInactive(thisSong));
        }
    }

    public Song CrossfadeInNewSong(AudioClip song, float duration, bool looping = false)
    {
        FadeOutAnySongs(duration);
        return FadeInNewSong(song, duration, looping);
    }

    private void Update()
    {
        for(int i = 0; i < ActiveSongs.Count; ++i)
        {
            ActiveSongs[i].CheckForSongEvent();
        }
    }
}