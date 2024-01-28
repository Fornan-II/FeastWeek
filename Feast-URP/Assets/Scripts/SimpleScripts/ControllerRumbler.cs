using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerRumbler : MonoBehaviour
{
    [Range(0, 1)] public float LowFrequencyRumble = 0f;
    [Range(0, 1)] public float HighFrequencyRumble = 0f;
    [SerializeField] private bool ignoreGamepadOnly = false;

    private bool motorsReset = true;

    private void OnDisable()
    {
        if (!motorsReset)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
            motorsReset = true;
        }
    }

    void Update()
    {
        if(GameManager.Instance.UsingGamepadControls() || ignoreGamepadOnly)
        {
            Gamepad.current.SetMotorSpeeds(LowFrequencyRumble, HighFrequencyRumble);
            motorsReset = false;
        }
        else if(!motorsReset && Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
            motorsReset = true;
        }
    }
}
