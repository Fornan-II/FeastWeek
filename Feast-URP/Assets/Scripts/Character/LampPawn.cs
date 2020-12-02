using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class LampPawn : VehiclePawn
{
    [SerializeField] private Transform lookTransform;
    [SerializeField] private float lookSpeed = 0.25f;
    [SerializeField] private float clampAngle = 60f;

    private Vector2 _lookInput;

    private void OnLook(InputValue input) => _lookInput = input.Get<Vector2>();
    private void OnWalk(InputValue input) => ReturnControl();
    private void OnJump(InputValue input) => ReturnControl();
    private void OnInteract(InputValue input) => ReturnControl();

    public override UnityAction BecomeControlledBy(Controller controller)
    {
        MsgBox.ShowMessage("Mouse to control", 3f);
        return base.BecomeControlledBy(controller);
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
