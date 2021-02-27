using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private const string k_LookSensitivity_Float = "Look Sensitivity - Float";
    private const string k_Brightness_Float = "Brightness - Float";
    private const string k_FullScreen_Int = "Fullscreen - Int";
    private const string k_Resolution_Int = "Resolution - Int";

    private const string k_VolumeMaster_Float = "Volume: Master - Float";
    private const string k_VolumeSFX_Float = "Volume: SFX - Float";
    private const string k_VolumeAmbient_Float = "Volume: Ambient - Float";
    private const string k_VolumeMusic_Float = "Volume: Music - Float";
    private const string k_VolumeUI_Float = "Volume: UI - Float";
    private const string k_Mixer_VolumeMaster = "VolumeMaster";
    private const string k_Mixer_VolumeSFX = "VolumeSFX";
    private const string k_Mixer_VolumeAmbient = "VolumeAmbient";
    private const string k_Mixer_VolumeMusic = "VolumeMusic";
    private const string k_Mixer_VolumeUI = "VolumeUI";

    [Header("UI Elements")]
    [SerializeField] private Slider lookSensitivitySlider;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Dropdown fullScreenDropdown;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Slider volumeMasterSlider;
    [SerializeField] private Slider volumeSFXSlider;
    [SerializeField] private Slider volumeAmbientSlider;
    [SerializeField] private Slider volumeMusicSlider;
    [SerializeField] private Slider volumeUISlider;

    private float _lookSensitivity;
    private float _brightness;
    private FullScreenMode _fullscreen;
    private int _resolutionIndex;   // of Screen.resolutions
    private float _volumeMaster;
    private float _volumeSFX;
    private float _volumeAmbient;
    private float _volumeMusic;
    private float _volumeUI;

    private void Start()
    {
        GetSettingValues();
    }

    #region Misc Public Methods
    public void SaveAndApply()
    {
        SaveSettings();
        ApplyAll();
    }

    public void ResetToSavedValues()
    {
        GetSettingValues();
        ApplyAll();
    }
    #endregion

    #region UI Receiving Methods
    public void SetLookSenstivity(float value)
    {
        _lookSensitivity = value;
    }

    public void SetBrightness(float value)
    {
        _brightness = value;
    }

    // FullScreen dropdown
    
    // Resolution dropdown

    public void SetVolumeMaster(float value)
    {
        _volumeMaster = value;
        ApplyAudioVolumes();
    }

    public void SetVolumeSFX(float value)
    {
        _volumeSFX = value;
        ApplyAudioVolumes();
    }

    public void SetVolumeAmbient(float value)
    {
        _volumeAmbient = value;
        ApplyAudioVolumes();
    }

    public void SetVolumeMusic(float value)
    {
        _volumeMusic = value;
        ApplyAudioVolumes();
    }

    public void SetVolumeUI(float value)
    {
        _volumeUI = value;
        ApplyAudioVolumes();
    }
    #endregion

    #region Settings Internal Loading/Saving/Applying
    private void GetSettingValues()
    {
        // Look sensitivity
        if (PlayerPrefs.HasKey(k_LookSensitivity_Float))
            _brightness = PlayerPrefs.GetFloat(k_LookSensitivity_Float);
        else
            _brightness = 1f;

        // Brightness
        if (PlayerPrefs.HasKey(k_Brightness_Float))
            _brightness = PlayerPrefs.GetFloat(k_Brightness_Float);
        else
            _brightness = 1f;

        // Fullscreen
        if (PlayerPrefs.HasKey(k_FullScreen_Int))
            _fullscreen = (FullScreenMode)PlayerPrefs.GetInt(k_FullScreen_Int);
        else
            _fullscreen = Screen.fullScreenMode;

        // Resolution
        if (PlayerPrefs.HasKey(k_Resolution_Int))
            _resolutionIndex = PlayerPrefs.GetInt(k_Resolution_Int);
        else
        {
            bool searchingForIndex = true;
            for(int i = 0; i < Screen.resolutions.Length && searchingForIndex; ++i)
            {
                if(Screen.resolutions[i].Equals(Screen.currentResolution))
                {
                    searchingForIndex = false;
                    _resolutionIndex = i;
                }
            }
        }

        // Volume Master
        if (PlayerPrefs.HasKey(k_VolumeMaster_Float))
            _volumeMaster = PlayerPrefs.GetFloat(k_VolumeMaster_Float);
        else
            AudioManager.Data.Mixer.GetFloat(k_Mixer_VolumeMaster, out _volumeMaster);

        // Volume SFX
        if (PlayerPrefs.HasKey(k_VolumeSFX_Float))
            _volumeSFX = PlayerPrefs.GetFloat(k_VolumeSFX_Float);
        else
            AudioManager.Data.Mixer.GetFloat(k_Mixer_VolumeSFX, out _volumeSFX);

        // Volume Ambient
        if (PlayerPrefs.HasKey(k_VolumeAmbient_Float))
            _volumeAmbient = PlayerPrefs.GetFloat(k_VolumeAmbient_Float);
        else
            AudioManager.Data.Mixer.GetFloat(k_Mixer_VolumeAmbient, out _volumeAmbient);

        // Volume Music
        if (PlayerPrefs.HasKey(k_VolumeMusic_Float))
            _volumeMusic = PlayerPrefs.GetFloat(k_VolumeMusic_Float);
        else
            AudioManager.Data.Mixer.GetFloat(k_Mixer_VolumeMusic, out _volumeMusic);

        // Volume UI
        if (PlayerPrefs.HasKey(k_VolumeUI_Float))
            _volumeUI = PlayerPrefs.GetFloat(k_VolumeUI_Float);
        else
            AudioManager.Data.Mixer.GetFloat(k_Mixer_VolumeUI, out _volumeUI);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(k_LookSensitivity_Float, _lookSensitivity);
        PlayerPrefs.SetFloat(k_Brightness_Float, _brightness);
        PlayerPrefs.SetInt(k_FullScreen_Int, (int)_fullscreen);
        PlayerPrefs.SetInt(k_Resolution_Int, _resolutionIndex);
        PlayerPrefs.SetFloat(k_VolumeMaster_Float, _volumeMaster);
        PlayerPrefs.SetFloat(k_VolumeSFX_Float, _volumeSFX);
        PlayerPrefs.SetFloat(k_VolumeAmbient_Float, _volumeAmbient);
        PlayerPrefs.SetFloat(k_VolumeMusic_Float, _volumeMusic);
        PlayerPrefs.SetFloat(k_VolumeUI_Float, _volumeUI);

        PlayerPrefs.Save();
    }

    private void ApplyAll()
    {
        // _lookSensitivity
        // _brightness

        Screen.SetResolution(
            Screen.resolutions[_resolutionIndex].width,
            Screen.resolutions[_resolutionIndex].height,
            _fullscreen,
            Screen.resolutions[_resolutionIndex].refreshRate);

        ApplyAudioVolumes();
    }

    private void ApplyAudioVolumes()
    {
        // I want to be able to scale volume properties to be more meaningful.
        // Saw a post about it in the past, can't quite remember... something about logarithmic scale maybe.
        AudioManager.Data.Mixer.SetFloat(k_Mixer_VolumeMaster, _volumeMaster);
        AudioManager.Data.Mixer.SetFloat(k_Mixer_VolumeSFX, _volumeSFX);
        AudioManager.Data.Mixer.SetFloat(k_Mixer_VolumeAmbient, _volumeAmbient);
        AudioManager.Data.Mixer.SetFloat(k_Mixer_VolumeMusic, _volumeMusic);
        AudioManager.Data.Mixer.SetFloat(k_Mixer_VolumeUI, _volumeUI);
    }

    private void SetUIValues()
    {
        lookSensitivitySlider.value = _lookSensitivity;
        brightnessSlider.value = _brightness;

        fullScreenDropdown.value = (int)_fullscreen;
        resolutionDropdown.value = _resolutionIndex;

        volumeMasterSlider.value = _volumeMaster;
        volumeSFXSlider.value = _volumeSFX;
        volumeAmbientSlider.value = _volumeAmbient;
        volumeMusicSlider.value = _volumeMusic;
        volumeUISlider.value = _volumeUI;
    }
    #endregion
}
