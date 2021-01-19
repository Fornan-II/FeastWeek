using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioCue : MonoBehaviour
{
    #region  Static Pool Management
    private const int MaxPooledCues = 100;

    private static List<AudioCue> _activeAudioCues = new List<AudioCue>();
    private static List<AudioCue> _inactiveAudioCues = new List<AudioCue>();
    private static Transform _audioCuePoolParent;

    public static AudioCue GetInactiveCue() => _inactiveAudioCues.Count > 0 ? _inactiveAudioCues[0] : CreateNew();

    public static AudioCue GetActiveCue()
    {
        AudioCue cue = GetInactiveCue();
        cue.SetActive();
        return cue;
    }

    private static AudioCue CreateNew()
    {
        if (!_audioCuePoolParent)
        {
            _audioCuePoolParent = new GameObject().transform;
            _audioCuePoolParent.name = "Audio Cue Pool";
            DontDestroyOnLoad(_audioCuePoolParent);
        }
        
        GameObject newGameObject = new GameObject();
        newGameObject.name = "Audio Cue";
        newGameObject.SetActive(false);
        newGameObject.transform.parent = _audioCuePoolParent;
        
        AudioCue newCue = newGameObject.AddComponent<AudioCue>();
        newCue.CreateAudioSource();
        _inactiveAudioCues.Add(newCue);
        return newCue;
    }

    public static void ResetCuePool(bool resetActiveCues = true)
    {
        Destroy(_audioCuePoolParent.gameObject);
        _inactiveAudioCues = null;

        if (!resetActiveCues) return;
        
        while (_activeAudioCues.Count > 0)
        {
            Destroy(_activeAudioCues[0]);
            _activeAudioCues.RemoveAt(0);
        }
    }
    #endregion

    #region CueSettings Struct
    [System.Serializable]
    public struct CueSettings
    {
        public CueSettings(AudioManager.MixerGroup group)
        {
            this = Default;
            Group = group;
        }
        
        public AudioManager.MixerGroup Group;
        public bool Loop;
        [Range(0,1)] public float Volume;
        public Vector2 PitchRange;
        public float MinDistance;
        public float MaxDistance;

        public float GetPitch() => Random.Range(PitchRange.x, PitchRange.y);

        public static CueSettings Default = new CueSettings(AudioManager.MixerGroup.Master)
        {
            Loop = false,
            Volume = 1f,
            PitchRange = Vector2.one,
            MinDistance = 1f,
            MaxDistance = 50f
        };
    }
    #endregion
    
    #region Variables

    protected AudioSource _source;
    public AudioSource Source => _source;
    public bool IsPlaying => _source.isPlaying;

    protected CueSettings _settings = CueSettings.Default;
    public CueSettings Settings
    {
        set
        {
            _settings = value;
            ApplySettings();
        }
        get => _settings;
    }
    public event UnityAction OnFinishedPlaying;
    #endregion

    #region Unity Methods

    private void Start()
    {
        if (!Source) CreateAudioSource();
    }

    private void OnDestroy()
    {
        Source.outputAudioMixerGroup = AudioManager.Data.Mixer.outputAudioMixerGroup;
        _activeAudioCues.Remove(this);
        _inactiveAudioCues.Remove(this);
    }

    private void Update()
    {
        if (!Source.isPlaying)
        {
            OnFinishedPlaying?.Invoke();
            OnFinishedPlaying = null;
            
            SetInactive();
        }
    }
    #endregion

    #region Methods
    public void SetActive()
    {
        _inactiveAudioCues.Remove(this);
        _activeAudioCues.Add(this);
        if (transform.parent == _audioCuePoolParent)
        {
            transform.parent = null;
            Util.UndoDontDestroyOnLoad(gameObject);
        }

        gameObject.SetActive(true);
    }

    public void SetInactive()
    {
        if (_inactiveAudioCues.Count + _activeAudioCues.Count < MaxPooledCues)
        {
            _activeAudioCues.Remove(this);
            _inactiveAudioCues.Add(this);
            ResetClip();
            gameObject.SetActive(false);
            transform.parent = _audioCuePoolParent;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void Play() => Source.Play();
    public void Stop() => Source.Stop();
    public void SetClip(AudioClip clip) => Source.clip = clip;
    public void ResetClip() => Source.clip = null;

    protected void CreateAudioSource()
    {
        _source = gameObject.AddComponent<AudioSource>();
        _source.playOnAwake = false;
        //spatialBlend = 1 makes the audio 3D
        _source.spatialBlend = 1f;
        _source.rolloffMode = AudioRolloffMode.Custom;
        _source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, AudioManager.Data.DefaultRollOffCurve);
        
        ApplySettings();
    }
    
    protected void ApplySettings()
    {
        _source.outputAudioMixerGroup = AudioManager.GetGroup(_settings.Group);
        _source.loop = _settings.Loop;
        _source.volume = _settings.Volume;
        _source.pitch = _settings.GetPitch();
        
        // Set up correct roll off
        _source.minDistance = _settings.MinDistance;
        _source.maxDistance = _settings.MaxDistance;
    }
    #endregion
}
