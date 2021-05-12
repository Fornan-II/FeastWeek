using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private Button applyButton;
    [SerializeField] private Slider lookSensitivitySlider;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TMP_Dropdown fullScreenDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider volumeMasterSlider;
    [SerializeField] private Slider volumeSFXSlider;
    [SerializeField] private Slider volumeAmbientSlider;
    [SerializeField] private Slider volumeMusicSlider;
    [SerializeField] private Slider volumeUISlider;

    public static float LookSensitivity { get; private set; } = 1f;

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
        SetUIValues();
    }

    #region Misc Public Methods
    public void SaveAndApply()
    {
        SaveSettings();
        ApplyAll();
        applyButton.gameObject.SetActive(false);
    }

    public void ResetToSavedValues()
    {
        GetSettingValues();
        ApplyAll();
        applyButton.gameObject.SetActive(false);
    }
    #endregion

    #region UI Receiving Methods
    public void SetLookSenstivity(float value)
    {
        LookSensitivity = value;
        applyButton.gameObject.SetActive(true);
    }

    public void SetBrightness(float value)
    {
        _brightness = value;
        applyButton.gameObject.SetActive(true);
    }

    public void SetFullscreen(int value)
    {
        _fullscreen = (FullScreenMode)value;
        applyButton.gameObject.SetActive(true);
    }
    
    public void SetResolution(int value)
    {
        _resolutionIndex = value;
        applyButton.gameObject.SetActive(true);
    }

    public void SetVolumeMaster(float value)
    {
        _volumeMaster = value;
        ApplyAudioVolumes();
        applyButton.gameObject.SetActive(true);
    }

    public void SetVolumeSFX(float value)
    {
        _volumeSFX = value;
        ApplyAudioVolumes();
        applyButton.gameObject.SetActive(true);
    }

    public void SetVolumeAmbient(float value)
    {
        _volumeAmbient = value;
        ApplyAudioVolumes();
        applyButton.gameObject.SetActive(true);
    }

    public void SetVolumeMusic(float value)
    {
        _volumeMusic = value;
        ApplyAudioVolumes();
        applyButton.gameObject.SetActive(true);
    }

    public void SetVolumeUI(float value)
    {
        _volumeUI = value;
        ApplyAudioVolumes();
        applyButton.gameObject.SetActive(true);
    }
    #endregion

    #region Settings Internal Loading/Saving/Applying
    private void GetSettingValues()
    {
        // Look sensitivity
        if (PlayerPrefs.HasKey(k_LookSensitivity_Float))
            LookSensitivity = PlayerPrefs.GetFloat(k_LookSensitivity_Float);
        else
            LookSensitivity = 1f;

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
            _volumeMaster = 1.0f;

        // Volume SFX
        if (PlayerPrefs.HasKey(k_VolumeSFX_Float))
            _volumeSFX = PlayerPrefs.GetFloat(k_VolumeSFX_Float);
        else
            _volumeSFX = 1.0f;

        // Volume Ambient
        if (PlayerPrefs.HasKey(k_VolumeAmbient_Float))
            _volumeAmbient = PlayerPrefs.GetFloat(k_VolumeAmbient_Float);
        else
            _volumeAmbient = 1.0f;

        // Volume Music
        if (PlayerPrefs.HasKey(k_VolumeMusic_Float))
            _volumeMusic = PlayerPrefs.GetFloat(k_VolumeMusic_Float);
        else
            _volumeMusic = 1.0f;

        // Volume UI
        if (PlayerPrefs.HasKey(k_VolumeUI_Float))
            _volumeUI = PlayerPrefs.GetFloat(k_VolumeUI_Float);
        else
            _volumeUI = 1.0f;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(k_LookSensitivity_Float, LookSensitivity);
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
        AudioManager.Data.Mixer.SetFloat(k_Mixer_VolumeMaster, RemapVolumePercentToDecibel(_volumeMaster));
        AudioManager.Data.Mixer.SetFloat(k_Mixer_VolumeSFX, RemapVolumePercentToDecibel(_volumeSFX));
        AudioManager.Data.Mixer.SetFloat(k_Mixer_VolumeAmbient, RemapVolumePercentToDecibel(_volumeAmbient));
        AudioManager.Data.Mixer.SetFloat(k_Mixer_VolumeMusic, RemapVolumePercentToDecibel(_volumeMusic));
        AudioManager.Data.Mixer.SetFloat(k_Mixer_VolumeUI, RemapVolumePercentToDecibel(_volumeUI));
    }

    private void SetUIValues()
    {
        lookSensitivitySlider.value = LookSensitivity;
        brightnessSlider.value = _brightness;

        List<string> optionData;
        
        fullScreenDropdown.ClearOptions();
        optionData = new List<string>() { "Exclusive Full Screen", "Full Screen Window", "Maximized Window", "Windowed"  };
        fullScreenDropdown.AddOptions(optionData);
        optionData.Clear();
        fullScreenDropdown.value = (int)_fullscreen;
        fullScreenDropdown.RefreshShownValue();

        resolutionDropdown.ClearOptions();
        foreach (var resolution in Screen.resolutions)
        {
            optionData.Add(string.Format("{0}x{1} {2}hz", resolution.width, resolution.height, resolution.refreshRate));
        }
        resolutionDropdown.AddOptions(optionData);
        optionData.Clear();
        resolutionDropdown.value = _resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        volumeMasterSlider.value = _volumeMaster;
        volumeSFXSlider.value = _volumeSFX;
        volumeAmbientSlider.value = _volumeAmbient;
        volumeMusicSlider.value = _volumeMusic;
        volumeUISlider.value = _volumeUI;
    }
    #endregion

    // Converte a 0 to 1 value to decibels, which is NOT a linear scale
    // https://stackoverflow.com/questions/31598410/how-can-i-normalized-decibel-value-and-make-it-between-0-and-1
    // Also handling when t is less than zero and would return negative infinity
    // by setting it at the lowest volume (-80)
    private float RemapVolumePercentToDecibel(float t) => t <= 0 ? -80f : 20.0f * Mathf.Log10(t);
}
