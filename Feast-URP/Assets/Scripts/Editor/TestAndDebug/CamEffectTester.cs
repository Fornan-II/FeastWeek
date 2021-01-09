using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamEffectTester : MonoBehaviour
{
    public bool TestImpulse = true;
    public bool TestShake = true;
    [Header("Impulse settings")]
    [SerializeField] private float ImpulseStrength = 1f;
    [SerializeField] private Transform ImpulseSource;
    [SerializeField] private float ImpulseDuration = 0.5f;
    [Header("Screen Shake settings")]
    [SerializeField] private float ScreenShakeStrength = 1f;
    [SerializeField] private float ScreenShakeDuration = 0.5f;

    private bool _wasPressed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.rKey.isPressed && !_wasPressed)
        {
            if(TestImpulse)
            {
                MainCamera.Effects.ApplyImpulse(ImpulseSource ? ImpulseSource.position : transform.position, ImpulseStrength, ImpulseDuration);
            }
            if(TestShake)
            {
                MainCamera.Effects.ApplyScreenShake(ScreenShakeStrength, ScreenShakeDuration);
            }
        }

        _wasPressed = Keyboard.current.rKey.isPressed;
    }
}
