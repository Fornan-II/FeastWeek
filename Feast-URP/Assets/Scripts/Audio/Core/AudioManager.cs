using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioManager
{
    // Searches in folders named "Resources"
    private const string k_MixerFilePath = "AudioManager Data";

    public enum MixerGroup
    {
        Master = 0,
        SFX = 1,
        SFX_Ducking = 5,
        Ambient = 2,
        Ambient_LPF = 4,
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
        
        Data = Resources.Load<AudioManagerData>(k_MixerFilePath);
        //Debug.Log($"Hey it's init time, and we loaded {Data.Mixer}.");
        
        _mixerGroups = new Dictionary<MixerGroup, AudioMixerGroup>()
        {
            {MixerGroup.Master, Data.Mixer.FindMatchingGroups("Master")[0]},
            {MixerGroup.SFX, Data.Mixer.FindMatchingGroups("SFX")[0]},
            {MixerGroup.SFX_Ducking, Data.Mixer.FindMatchingGroups("SFX-Ducking")[0]},
            {MixerGroup.Ambient, Data.Mixer.FindMatchingGroups("Ambient")[0]},
            {MixerGroup.Ambient_LPF, Data.Mixer.FindMatchingGroups("Ambient-LPF")[0] },
            {MixerGroup.Music, Data.Mixer.FindMatchingGroups("Music")[0]}
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
        AudioCue cue = PlaySound(clip, followTransform.position, settings);
        cue.transform.parent = followTransform;
        return cue;
    }

    public static AudioMixerGroup GetGroup(MixerGroup group) => _mixerGroups[group];
}
