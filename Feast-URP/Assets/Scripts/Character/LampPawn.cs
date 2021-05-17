using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class LampPawn : VehiclePawn, DefaultControls.IFPSCharacterActions
{
#pragma warning disable 0649
    [SerializeField] private Transform lookTransform;
    [SerializeField] private float lookSpeed = 0.25f;
    [SerializeField] private float clampAngle = 60f;

    private Vector2 _lookInput;

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
    public void OnJump(InputAction.CallbackContext context) => ReturnControl();
    public void OnSprint(InputAction.CallbackContext context) { /* Do nothing */ }
    public void OnInteract(InputAction.CallbackContext context) => ReturnControl();
    #endregion

    public override UnityAction BecomeControlledBy(Controller controller)
    {
        MsgBox.ShowMessage("Mouse to control", 3f);
        return base.BecomeControlledBy(controller);
    }

    protected override void StopBeingControlled()
    {
        base.StopBeingControlled();
        _lookInput = Vector2.zero;
    }

    private void Update()
    {
        if (!IsBeingControlled || Time.timeScale <= 0f) return;

        // Rotate
        Vector2 lookCalc = _lookInput * lookSpeed;
        lookTransform.rotation *= Quaternion.Euler(-lookCalc.y, lookCalc.x, 0);

        // Clamp
        Quaternion forwardQuaternion = Quaternion.Euler(Vector3.forward) * Quaternion.Euler(0, lookTransform.localEulerAngles.y, 0f);
        if (Quaternion.Angle(forwardQuaternion, lookTransform.localRotation) > clampAngle)
        {
            lookTransform.localRotation = Quaternion.RotateTowards(forwardQuaternion, lookTransform.localRotation, clampAngle);
        }

        // Stop roll
        Vector3 localRotation = lookTransform.localEulerAngles;
        localRotation.z = 0f;
        lookTransform.localRotation = Quaternion.Euler(localRotation);
    }
}
