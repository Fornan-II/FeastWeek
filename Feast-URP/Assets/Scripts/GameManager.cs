using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (!_initialized) CreateInstance();
            return _instance;
        }
    }

    private static bool _initialized = false;

    private static void CreateInstance()
    {
        _initialized = true;
        _instance = new GameObject("~GameManager").AddComponent<GameManager>();
        DontDestroyOnLoad(_instance.gameObject);

        _instance.SetupControls();
    }

    public enum ControlSchemeType
    {
        INVALID,
        KEYBOARD_MOUSE,
        GAMEPAD
    }

    public DefaultControls Controls { get; private set; }
    public ControlSchemeType ActiveControlScheme { get; private set; } = ControlSchemeType.INVALID;
    public event Action OnControlSchemeChanged;

    private InputDevice _lastUsedDevice;
    
    private void OnDestroy()
    {
        foreach (var map in Controls.asset.actionMaps)
        {
            UnregisterActions(map);
        }

        _instance = null;
    }

    public bool UsingGamepadControls() => ActiveControlScheme == ControlSchemeType.GAMEPAD;

    public void RegisterActions(InputActionMap map)
    {
        map.actionTriggered += ActionMap_actionTriggered;
    }

    public void UnregisterActions(InputActionMap map)
    {
        map.actionTriggered -= ActionMap_actionTriggered;
    }

    private void SetupControls()
    {
        Controls = new DefaultControls();
        
        foreach (var map in Controls.asset.actionMaps)
        {
            RegisterActions(map);
        }
    }

    private void ActionMap_actionTriggered(InputAction.CallbackContext context)
    {
        // Workaround for detecting active control scheme
        // Based on this and other responses in this thread:
        // https://forum.unity.com/threads/detect-most-recent-input-device-type.753206/#post-5026484

        // This method gets called a lot because it's called for every input

        bool deviceIsDifferent = false;
        
        // Using a try catch here because the new Input System has a bug where during context switches it will trigger this callback and
        // send a context where context.control throws a IndexOutOfRangeException.
        try
        {
            deviceIsDifferent = _lastUsedDevice != context.control.device;
        }
        catch(IndexOutOfRangeException)
        {
            return;
        }

        if (deviceIsDifferent)
        {
            _lastUsedDevice = context.control.device;

            bool deviceFound = false;
            for (int i = 0; i < Controls.controlSchemes.Count && !deviceFound; ++i)
            {
                if(Controls.controlSchemes[i].SupportsDevice(_lastUsedDevice))
                {
                    deviceFound = true;
                    ControlSchemeType newControlScheme = ControlSchemeType.INVALID;

                    switch (Controls.controlSchemes[i].name)
                    {
                        case "Gamepad":
                            newControlScheme = ControlSchemeType.GAMEPAD;
                            break;
                        case "Keyboard and Mouse":
                            newControlScheme = ControlSchemeType.KEYBOARD_MOUSE;
                            break;
                        default:
                            Debug.LogErrorFormat("[GameManager] Unhandled control scheme {0}, cannot switch contexts!", Controls.controlSchemes[i].name);
                            break;
                    }

                    if(newControlScheme != ActiveControlScheme)
                    {
                        ActiveControlScheme = newControlScheme;
                        OnControlSchemeChanged?.Invoke();
                        return;
                    }
                }
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!deviceFound)
                Debug.LogError("[GameManager] Failed to handle ActionMap_actionTriggered callback.");
#endif
        }
    }
}
