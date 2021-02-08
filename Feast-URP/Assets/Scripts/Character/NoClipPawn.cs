using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoClipPawn : VehiclePawn
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float lookSpeed = 0.25f;

    private Vector2 _lookInput;
    private Vector2 _horizontalMovement;
    private float _verticalMovement;

    private void OnLook(InputValue input) => _lookInput = input.Get<Vector2>();
    private void OnMoveHorizontal(InputValue input) => _horizontalMovement = input.Get<Vector2>();
    private void OnMoveVertical(InputValue input) => _verticalMovement = input.Get<float>();

    private void Update()
    {
        if (!IsBeingControlled || Time.timeScale <= 0f) return;

        // Move
        transform.position += (transform.forward * _horizontalMovement.y + transform.right * _horizontalMovement.x + transform.up * _verticalMovement) * moveSpeed * Time.deltaTime;

        // Rotate
        Vector2 lookCalc = _lookInput * lookSpeed;
        transform.rotation *= Quaternion.Euler(-lookCalc.y, lookCalc.x, 0);

        // Clamp
        Quaternion forwardQuaternion = Quaternion.Euler(Vector3.forward) * Quaternion.Euler(0, transform.localEulerAngles.y, 0f);
        if (Quaternion.Angle(forwardQuaternion, transform.localRotation) > 90f)
        {
            transform.localRotation = Quaternion.RotateTowards(forwardQuaternion, transform.localRotation, 90f);
        }

        // Stop roll
        Vector3 localRotation = transform.localEulerAngles;
        localRotation.z = 0f;
        transform.localRotation = Quaternion.Euler(localRotation);
    }
}
