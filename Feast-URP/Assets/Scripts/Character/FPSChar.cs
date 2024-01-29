using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSChar : Pawn, ICheckpointUser, DefaultControls.IFPSCharacterActions
{
#pragma warning disable 0649
    public Transform LookTransform => lookTransform;

    [Header("Components")]
    [SerializeField] private CharacterController movementController;
    [SerializeField] private RaycastInteracter interacter;
    [SerializeField] private Transform lookTransform;
    [SerializeField] private FootstepPlayer footstepPlayer;
    [Header("Movement")]
    [SerializeField] private float moveGroundAccelLerp = 0.2f;
    [SerializeField] private float moveAerialAccelLerp = 0.07f;
    [SerializeField] private float lookSpeed = 0.25f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpForce = 5f;
    [Header("Ground Check")]
    [SerializeField] private float maxGroundAngle = 45f;
    [SerializeField] private float groundCheckDistance = 0.01f;
    [SerializeField, Range(0, 1)] private float groundCheckSensitivity = 0.95f;
    [SerializeField] private LayerMask groundCheckMask = Physics.AllLayers;
    [Header("Sound")]
    [SerializeField] private Vector2 footstepStrideDistance = new Vector2(0.0203f, 0.0392f);
    [SerializeField] private float minFootstepTime = 0.1f;
    [SerializeField] private float jumpVolumeMultiplier = 2f;
    [Header("Visual Flare")]
    [SerializeField] private AnimationCurve landingForceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private float timeCurveScaler = 2f;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _jumpInput;
    private bool _sprintInput;

    private bool _isGrounded;
    private Vector3 _groundNormal;
    private Transform _groundTransform;
    private FootstepSurface.SurfaceType _groundSurfaceType;

    private float _currentYVelocityMax = 0.0f;
    private float _distanceToNextFootstep = 0f;
    private float _lastFootstepTime = -1f;

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

    public void OnWalk(InputAction.CallbackContext context) => _moveInput = Util.RemapSquareToCircle(context.ReadValue<Vector2>());
    public void OnLook(InputAction.CallbackContext context) => _lookInput = Util.ScaleInputToScreen(context.ReadValue<Vector2>()) * SettingsManager.LookSensitivity;
    public void OnJump(InputAction.CallbackContext context) => _jumpInput = true;
    public void OnSprint(InputAction.CallbackContext context) => _sprintInput = !context.canceled;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && (PauseManager.Instance ? !PauseManager.Instance.IsPaused : true))
            interacter.TryInteract(this);
    }
    #endregion

    protected override void StopBeingControlled()
    {
        base.StopBeingControlled();
        _moveInput = Vector2.zero;
        _lookInput = Vector2.zero;
        _sprintInput = false;
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        movementController.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        movementController.enabled = true;
    }

    public void ApplyExternalForce(Vector3 externalForce)
    {
        _currentYVelocityMax = externalForce.y;
        movementController.Move(externalForce * Time.deltaTime);
    }

    public Vector3 GetVelocity() => movementController.velocity;

    // Update is called once per frame
    void Update()
    {
        if (!IsBeingControlled || Time.timeScale <= 0f) return;

        bool wasGrounded = _isGrounded;
        GroundCheck();
        if (_isGrounded && !wasGrounded) OnPlayerBecomeGrounded();

        PlayerMovement();
        PlayerLook();
    }

    private void OnPlayerBecomeGrounded()
    {
        // Play footstep sfx for landing on the ground
        _distanceToNextFootstep = Util.RandomInRange(footstepStrideDistance);
        AudioCue.CueSettings cueSettings = footstepPlayer.FootStepSoundSettings;
        cueSettings.Volume *= jumpVolumeMultiplier;
        footstepPlayer.PlayFootstep(_groundSurfaceType, cueSettings);

        // Apply some camera impulse to represent force of landing on the ground
        float strength = landingForceCurve.Evaluate(-movementController.velocity.y);

        if(strength > 0f)
        {
            MainCamera.Effects.ApplyImpulse(MainCamera.RootTransform.position + Vector3.up, strength, strength * timeCurveScaler);
        }
        //Debug.LogFormat("{0} => {1}", movementController.velocity.y, strength);
    }
    
    private void PlayerMovement()
    {
        // Planar Movement
        Vector3 moveCalc = (transform.forward * _moveInput.y + transform.right * _moveInput.x) * moveSpeed * (_sprintInput && _isGrounded && _moveInput.y > 0 ? sprintMultiplier : 1f);
        moveCalc = Vector3.ProjectOnPlane(moveCalc, _groundNormal);
        Debug.DrawRay(transform.position - Vector3.up * movementController.height * 0.5f, moveCalc, Color.yellow, 0.05f);
        moveCalc = Vector3.Lerp(new Vector3(movementController.velocity.x, 0f, movementController.velocity.z), moveCalc, _isGrounded ? moveGroundAccelLerp : moveAerialAccelLerp);
        Debug.DrawRay(transform.position - Vector3.up * movementController.height * 0.5f, moveCalc, Color.red, 0.05f);

        // Vertical Movement
        if (_isGrounded && _jumpInput)
        {
            // If can jump and trying to jump, jump! Applies no gravity this frame.
            moveCalc.y = jumpForce;
            _currentYVelocityMax = jumpForce;

            // Play footstep audio at scaled volume to simulate pushing off the ground with both feet
            AudioCue.CueSettings cueSettings = footstepPlayer.FootStepSoundSettings;
            cueSettings.Volume *= jumpVolumeMultiplier;
            footstepPlayer.PlayFootstep(_groundSurfaceType, cueSettings);
            // Setting _isGrounded to false since we're no longer grounded
            _isGrounded = false;
        }
        else
        {
            //Footstep audio
            _distanceToNextFootstep -= moveCalc.magnitude * Time.deltaTime;
            if (_distanceToNextFootstep <= 0f && _isGrounded)
            {
                if(Time.time >= _lastFootstepTime + minFootstepTime)
                {
                    _distanceToNextFootstep = Util.RandomInRange(footstepStrideDistance);
                    _lastFootstepTime = Time.time;
                    footstepPlayer.PlayFootstep(_groundSurfaceType);
                }
            }

            // Keep the player moving at their current vertical speed.
            // Apply gravity, unless grounded (prevents slowly sliding down slopes)
            // Adding to preserve yVelocity from moveCalc being projected on ground plane.
            // using Min() to avoid issue where movementController y velocity spikes to go up steps.
            _currentYVelocityMax = Mathf.Max(0, Mathf.Min(movementController.velocity.y, _currentYVelocityMax));
            moveCalc.y += Mathf.Min(movementController.velocity.y, _currentYVelocityMax);
            if (!_isGrounded)
                moveCalc += Physics.gravity * Time.deltaTime;
        }

        _jumpInput = false;
        
        movementController.Move(moveCalc * Time.deltaTime);
    }

    private void PlayerLook()
    {
        // Don't allow look rotation while camera is blending
        if (MainCamera.IsBlending) return;

        Vector2 lookCalc = _lookInput * lookSpeed;// * Time.deltaTime; // InputSystem mouse offsets are already time-scaled!
        transform.rotation *= Quaternion.Euler(0, lookCalc.x, 0);
        lookTransform.rotation *= Quaternion.Euler(-lookCalc.y, 0, 0);
        if (Quaternion.Angle(Quaternion.Euler(Vector3.forward), lookTransform.localRotation) > 90f)
        {
            lookTransform.localRotation = Quaternion.RotateTowards(Quaternion.Euler(Vector3.forward), lookTransform.localRotation, 90f);
        }

        // Ensure there is no roll rotation
        Vector3 euler = lookTransform.rotation.eulerAngles;
        euler.z = 0;
        lookTransform.rotation = Quaternion.Euler(euler);
    }

    private void GroundCheck()
    {
        _isGrounded = false;
        _groundNormal = Vector3.up;

        if (Physics.SphereCast(
            transform.TransformPoint(movementController.center + Vector3.down * (movementController.height * 0.5f - movementController.radius)),
            movementController.radius * groundCheckSensitivity,
            -transform.up,
            out RaycastHit hitInfo,
            1f - groundCheckSensitivity + groundCheckDistance + movementController.skinWidth,
            groundCheckMask,
            QueryTriggerInteraction.Ignore
            ))
        {
            if (Vector3.Angle(hitInfo.normal, Vector3.up) <= maxGroundAngle)
            {
                _isGrounded = true;
                _groundNormal = hitInfo.normal;

                if(hitInfo.transform != _groundTransform)
                {
                    _groundTransform = hitInfo.transform;
                    if(_groundTransform.TryGetComponent(out FootstepSurface surface))
                    {
                        _groundSurfaceType = surface.GetSurfaceType(hitInfo);
                    }
                    else
                    {
                        _groundSurfaceType = FootstepSurface.SurfaceType.UNKNOWN;
                    }
                }
                
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        movementController = movementController ?? GetComponent<CharacterController>();
        footstepPlayer = footstepPlayer ?? GetComponent<FootstepPlayer>();
    }

    private void OnDrawGizmos()
    {
        if(movementController)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawSphere(Vector3.up * (movementController.height * 0.5f - movementController.radius), movementController.radius);
            Gizmos.DrawSphere(Vector3.zero, movementController.radius);
            Gizmos.DrawSphere(Vector3.down * (movementController.height * 0.5f - movementController.radius), movementController.radius);

            Gizmos.color = _isGrounded ? Color.blue : Color.red;
            Gizmos.matrix = Matrix4x4.identity;

            Vector3 groundCheckStartPoint = transform.TransformPoint(movementController.center + Vector3.down * (movementController.height * 0.5f - movementController.radius));
            float groundCheckRadius = movementController.radius * groundCheckSensitivity;
            Gizmos.DrawWireSphere(groundCheckStartPoint, groundCheckRadius);
            Gizmos.DrawWireSphere(groundCheckStartPoint - transform.up * (1f - groundCheckSensitivity + groundCheckDistance + movementController.skinWidth), groundCheckRadius);
        }
    }
#endif
}
