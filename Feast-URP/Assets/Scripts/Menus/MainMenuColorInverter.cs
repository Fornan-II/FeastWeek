using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuColorInverter : MonoBehaviour
{
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private AudioClip themeSong;
    [SerializeField] private AudioCue.CueSettings themeSongSettings = AudioCue.CueSettings.Default;

    private void Start()
    {
        if (GlobalData.HasCompletedGame)
        {
            // Invert color
            MainCamera.Effects.SetColorInvert(true);

            // Applying this so color inversion applies to UI
            // Under normal conditions the PP makes text look bad so usually
            // isn't set to ScreenSpaceCamera
            menuCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            menuCanvas.worldCamera = MainCamera.Camera;

            // Play music
            AudioManager.PlaySound(themeSong, Vector3.zero, themeSongSettings);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        menuCanvas = menuCanvas ?? GetComponent<Canvas>();
    }
#endif
}
