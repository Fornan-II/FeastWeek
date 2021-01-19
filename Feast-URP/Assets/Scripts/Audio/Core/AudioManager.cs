using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioManager
{
    private const string MixerFilePath = "AudioManager Data";

    public enum MixerGroup
    {
        Master = 0,
        SFX = 1,
        Ambient = 2,
        Music = 3
    }
    public static AudioManagerData Data { private set; get; }
    private static Dictionary<MixerGroup, AudioMixerGroup> _mixerGroups;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying) return;
        #endif
        
        Data = Resources.Load<AudioManagerData>(MixerFilePath);
        //Debug.Log($"Hey it's init time, and we loaded {Data.Mixer}.");
        
        _mixerGroups = new Dictionary<MixerGroup, AudioMixerGroup>()
        {
            {MixerGroup.Master, Data.Mixer.FindMatchingGroups("Master")[0]},
            {MixerGroup.SFX, Data.Mixer.FindMatchingGroups("SFX")[0]},
            {MixerGroup.Ambient, Data.Mixer.FindMatchingGroups("Ambient")[0]},
            {MixerGroup.Music, Data.Mixer.FindMatchingGroups("Music")[0]},
        };
    }
    
    public static AudioCue PlaySound(AudioClip clip, Vector3 location, AudioCue.CueSettings settings)
    {
        AudioCue cue = AudioCue.GetActiveCue();

        cue.transform.position = location;
        cue.Settings = settings;
        cue.SetClip(clip);
        cue.Play();
        
        return cue;
    }

    public static AudioCue PlaySound(AudioClip clip, Transform followTransform, AudioCue.CueSettings settings)
    {
        AudioCue cue = AudioCue.GetActiveCue();

        cue.transform.parent = followTransform;
        cue.transform.localPosition = Vector3.zero;
        cue.Settings = settings;
        cue.SetClip(clip);
        cue.Play();
        
        return cue;
    }

    public static AudioMixerGroup GetGroup(MixerGroup group) => _mixerGroups[group];
}
