using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class LampPawn : VehiclePawn, DefaultControls.IFPSCharacterActions
{
#pragma warning disable 0649
    [Header("Components")]
    [SerializeField] private Transform lookTransform;
    [SerializeField] private DoorMechanic target;
    [SerializeField] private AudioSource rotationAudioSource;
    [SerializeField] private Interactable interactable;
    [SerializeField] private GhostTether tether;
    [SerializeField] private ParticleSystem passiveParticles;
    [Header("Swiveling Settings")]
    [SerializeField] private float lookSpeed = 0.25f;
    [SerializeField] private float clampAngle = 60f;
    [Header("Alignment Settings")]
    [SerializeField] private float alignmentCutoff = 0.85f;
    [SerializeField] private AnimationCurve sensitivityCurve = AnimationCurve.Linear(1f, 1f, 0.85f, 0.2f);
    [Header("Audio - Swiveling")]
    [SerializeField] private AnimationCurve swivelVolumeCurve;
    [SerializeField] private float swivelSmoothing = 0.3f;
    [SerializeField] private float minSwivelVolume = 0.0001f;
    [Header("Audio - Alignment")]
    [SerializeField] private AudioClip lampActiveSFX;
    [SerializeField] private AudioClip lampActiveDistortSFX;
    [SerializeField] private AudioCue.CueSettings lampActiveSFXSettings = AudioCue.CueSettings.Default;
    [SerializeField] private float lampActiveSFXFadeOutDuration = 0.7f;
    [SerializeField] private ADSRCurve alignmentSFXADSR;
    [Header("Connection Broken")]
    [SerializeField] private AudioClip onConnectionBrokenSFX;
    [SerializeField] private AudioCue.CueSettings onConnectionBrokenSFXSettings = AudioCue.CueSettings.Default;
    [SerializeField] private ParticleSystem onConnectionBrokenParticles;
    [SerializeField] private UnityEvent onTetherBreak;
    [SerializeField] private AnimationCurve cameraNoiseBreakAnim;

    private Vector2 _lookInput;
    private float _swivelSmoothVelocity = 0.0f;
    private bool _isConnectedToTarget = true;
    private AudioCue _lampActiveCue;
    private AudioCue _lampActiveDistortCue;

    #region Unity Methods
    private void Start()
    {
        _lampActiveCue = AudioManager.PlaySound(lampActiveSFX, lookTransform.transform.position, lampActiveSFXSettings);
        _lampActiveDistortCue = AudioManager.PlaySound(lampActiveDistortSFX, lookTransform.transform.position, lampActiveSFXSettings);
        _lampActiveDistortCue.SetVolume(0f, false);
        target?.AddConnectedLamp(this);
    }

    private void Update()
    {
        // Handle swivel audio
        if (IsBeingControlled || rotationAudioSource.volume > 0)
        {
            float newSwivelVolume = Mathf.SmoothDamp(rotationAudioSource.volume, swivelVolumeCurve.Evaluate(_lookInput.sqrMagnitude), ref _swivelSmoothVelocity, swivelSmoothing);

            // SmoothDamp doesn't seem to converge to 0 so let's help it out
            if (newSwivelVolume < minSwivelVolume)
                newSwivelVolume = 0f;

            if (newSwivelVolume > 0 && rotationAudioSource.volume <= 0f)
            {
                rotationAudioSource.Play();
            }
            else if (newSwivelVolume <= 0 && rotationAudioSource.volume > 0f)
            {
                rotationAudioSource.Pause();
            }

            rotationAudioSource.volume = newSwivelVolume;
        }

        // Check that this is being controlled and that the game is not paused
        if (!IsBeingControlled || Time.timeScale <= 0f) return;

        Vector3 vecToTarget = (target.GetTargetPosition() - lookTransform.position).normalized;
        float dot = Vector3.Dot(lookTransform.forward, vecToTarget);

        // Rotate lamp based on input
        {
            Vector2 lookCalc = _lookInput * lookSpeed;

            if (_isConnectedToTarget)
            {
                //lookCalc *= sensitivityCurve.Evaluate(dot);
                float dotFactor = sensitivityCurve.Evaluate(dot);
                lookCalc *= dotFactor;
                AudioCueEffects.Mix(_lampActiveCue, _lampActiveDistortCue, dotFactor);
            }

            lookTransform.rotation *= Quaternion.Euler(-lookCalc.y, lookCalc.x, 0);
            ConstrainLampRotation();
        }

        // Check lamp's connection to target
        if(_isConnectedToTarget && dot < alignmentCutoff)
        {
            BreakTether();
        }
    }
    #endregion

    #region Input
    protected override void ActivateInput()
    {
        GameManager.Instance.Controls.FPSCharacter.SetCallbacks(this);
        GameManager.Instance.Controls.FPSCharacter.Enable();
    }

    protected override void DeactivateInput()
    {
        GameManager.Instance.Controls.FPSCharacter.SetCallbacks(null);
        GameManager.Instance.Controls.FPSCharacter.Disable();
    }

    public void OnWalk(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().sqrMagnitude > Mathf.Epsilon)
            ReturnControl();
    }

    public void OnLook(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>() * SettingsManager.LookSensitivity;
    public void OnJump(InputAction.CallbackContext context) { if (!PauseManager.Instance.IsPaused) ReturnControl(); }
    public void OnSprint(InputAction.CallbackContext context) { /* Do nothing */ }
    public void OnInteract(InputAction.CallbackContext context) { if (!PauseManager.Instance.IsPaused) ReturnControl(); }
    #endregion

    #region Pawn/Controller control
    public override UnityAction BecomeControlledBy(Controller controller)
    {
        MsgBox.GetInstance(MsgBox.MsgBoxType.ToolTip).ShowMessage(GameManager.Instance.UsingGamepadControls()
            ? "Right stick to control"
            : "Mouse to control",
            3f);
        return base.BecomeControlledBy(controller);
    }

    protected override void StopBeingControlled()
    {
        base.StopBeingControlled();
        _lookInput = Vector2.zero;
    }
    #endregion

    private void ConstrainLampRotation()
    {
        // Clamp pitch
        Quaternion forwardQuaternion = Quaternion.Euler(Vector3.forward) * Quaternion.Euler(0, lookTransform.localEulerAngles.y, 0f);
        if (Quaternion.Angle(forwardQuaternion, lookTransform.localRotation) > clampAngle)
        {
            lookTransform.localRotation = Quaternion.RotateTowards(forwardQuaternion, lookTransform.localRotation, clampAngle);
        }

        // Remove roll
        Vector3 localRotation = lookTransform.localEulerAngles;
        localRotation.z = 0f;
        lookTransform.localRotation = Quaternion.Euler(localRotation);
    }

    private void BreakTether()
    {
        _isConnectedToTarget = false;
        target.RemoveConnectedLamp(this);
        AudioManager.PlaySound(onConnectionBrokenSFX, lookTransform.transform.position, onConnectionBrokenSFXSettings);
        _lampActiveCue.FadeOut(lampActiveSFXFadeOutDuration);
        _lampActiveDistortCue.FadeOut(lampActiveSFXFadeOutDuration);
        _lampActiveCue = null;
        _lampActiveDistortCue = null;
        passiveParticles.Stop();

        tether.BreakChain();
        tether.AddTetherDissolveCompleteListener(ReturnControl);

        onTetherBreak.Invoke();

        interactable.IsInteractable = false;

        MainCamera.Effects.ApplyImpulse(lookTransform.position + Vector3.up, 0.25f);
        MainCamera.Effects.ApplyScreenShake(0.1f, 7f, 1f);
        StartCoroutine(CameraNoiseBurst());

        onConnectionBrokenParticles.Play();
    }

    private IEnumerator CameraNoiseBurst()
    {
        for(float timer = 0.0f; timer <= Util.AnimationCurveLengthTime(cameraNoiseBreakAnim); timer += Time.deltaTime)
        {
            MainCamera.Effects.ApplyCameraNoise(GetInstanceID(), cameraNoiseBreakAnim.Evaluate(timer));
            yield return null;
        }
        MainCamera.Effects.RemoveCameraNoise(GetInstanceID());
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {   
        if(lookTransform && target)
        {
            Vector3 vecToTarget = (target.GetTargetPosition() - lookTransform.position).normalized;
            Vector3 tangentToTarget = Vector3.Cross(vecToTarget, Vector3.up);
            float dot = Vector3.Dot(lookTransform.forward, vecToTarget);
            float theta = Mathf.Acos(alignmentCutoff);

            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireArc(lookTransform.position + vecToTarget * Mathf.Cos(theta), vecToTarget, tangentToTarget, 360f, Mathf.Sin(theta));
            
            Gizmos.color = dot >= alignmentCutoff ? Color.green : Color.blue;
            Gizmos.DrawRay(lookTransform.position, lookTransform.forward * 2f);
        }
    }

    [ContextMenu("Align to target")]
    private void AlignToTarget()
    {
        if(!(lookTransform && target))
        {
            Debug.LogError("Failed to align lamp. Please check that both lookTransform and target are assigned in the inspector.");
            return;
        }
        
        UnityEditor.Undo.RecordObject(lookTransform, "Align lamp to target");
        lookTransform.transform.forward = target.GetTargetPosition() - lookTransform.position;
        ConstrainLampRotation();
    }
#endif
}
