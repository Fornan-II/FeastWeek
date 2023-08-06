using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioCue : MonoBehaviour
{
    #region  Static Pool Management
    private const int k_MaxPooledCues = 100;

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
        [Range(0, 1)] public float Volume;
        public Vector2 PitchRange;
        public float MinDistance;
        public float MaxDistance;
        public bool Is3D;

        public float GetPitch() => Random.Range(PitchRange.x, PitchRange.y);

        public static CueSettings Default = new CueSettings(AudioManager.MixerGroup.Master)
        {
            Loop = false,
            Volume = 1f,
            PitchRange = Vector2.one,
            MinDistance = 1f,
            MaxDistance = 50f,
            Is3D = true
        };
    }
    #endregion

    #region Variables

    protected AudioSource _source;
    protected Coroutine _activeFadeRoutine;
    public AudioSource Source => _source;
    public bool IsPlaying => _source.isPlaying;
    public bool IsPaused { get; protected set; } = false;
    public bool IsFading => _activeFadeRoutine != null;


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
        if (!IsPlaying && !IsPaused)
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
        if (_inactiveAudioCues.Count + _activeAudioCues.Count < k_MaxPooledCues)
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

    public void Play()
    {
        if(IsPaused)
        {
            Source.UnPause();
            IsPaused = false;
        }
        else
        {
            Source.Play();
        }
    }

    public void Pause()
    {
        Source.Pause();
        IsPaused = true;
    }

    public void Stop()
    {
        Source.Stop();
        SetInactive();
    }

    public void SetClip(AudioClip clip) => Source.clip = clip;
    public void ResetClip() => Source.clip = null;

    public void SetVolume(float value, bool saveToSettings = true)
    {
        if(saveToSettings)
            _settings.Volume = value;
        _source.volume = value;
    }

    public void FadeIn(float targetVolume, float duration, UnityAction OnFadeComplete = null)
    {
        if (IsFading)
            StopCoroutine(_activeFadeRoutine);
        _activeFadeRoutine = StartCoroutine(FadeAudio(0f, targetVolume, duration, OnFadeComplete));
    }
    public void FadeOut(float duration, UnityAction OnFadeComplete = null)
    {
        if (IsFading)
            StopCoroutine(_activeFadeRoutine);
        _activeFadeRoutine = StartCoroutine(FadeAudio(Settings.Volume, 0f, duration, OnFadeComplete));
    }

    protected IEnumerator FadeAudio(float initial, float target, float duration, UnityAction OnFadeComplete)
    {
        SetVolume(initial);

        // Using unscaledDeltaTime because AudioSources should not be affected by Time.timeScale
        for (float timer = 0.0f; timer < duration; timer += Time.unscaledDeltaTime)
        {
            yield return null;
            float t = timer / duration;
            SetVolume(Mathf.Lerp(initial, target, t * t));
        }

        SetVolume(target);
        OnFadeComplete?.Invoke();
        _activeFadeRoutine = null;

        if (Settings.Volume <= 0)
            Stop();
    }

    protected void CreateAudioSource()
    {
        _source = gameObject.AddComponent<AudioSource>();
        _source.playOnAwake = false;
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
        _source.spatialBlend = _settings.Is3D ? 1 : 0;
        
        // Set up correct roll off
        _source.minDistance = _settings.MinDistance;
        _source.maxDistance = _settings.MaxDistance;
    }
    #endregion
}
