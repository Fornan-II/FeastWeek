using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuColorInverter : MonoBehaviour
{
    [SerializeField] private Canvas menuCanvas;

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
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        menuCanvas = menuCanvas ?? GetComponent<Canvas>();
    }
#endif
}
