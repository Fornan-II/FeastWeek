using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject GamepadControls;
    [SerializeField] private GameObject KeyboardMouseControls;

    private GameObject _activeControls;

    void Start()
    {
        OnConstrolSchemeChanged();
        GameManager.Instance.OnControlSchemeChanged += OnConstrolSchemeChanged;
    }

    private void OnDestroy()
    {
        if(GameManager.Instance)
        {
            GameManager.Instance.OnControlSchemeChanged -= OnConstrolSchemeChanged;
        }
    }

    private void OnConstrolSchemeChanged()
    {
        switch (GameManager.Instance.ActiveControlScheme)
        {
            case GameManager.ControlSchemeType.INVALID:
            case GameManager.ControlSchemeType.KEYBOARD_MOUSE:
                _activeControls?.SetActive(false);
                _activeControls = KeyboardMouseControls;
                _activeControls.SetActive(true);
                break;
            case GameManager.ControlSchemeType.GAMEPAD:
                _activeControls?.SetActive(false);
                _activeControls = GamepadControls;
                _activeControls.SetActive(true);
                break;
        }
    }
}
