using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSChar : Pawn
{
#pragma warning disable 0649
    [Header("Components")]
    [SerializeField] private Transform lookTransform;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider col;
    [Header("Movement")]
    [SerializeField] private float moveGroundAccelLerp = 0.2f;
    [SerializeField] private float moveAerialAccelLerp = 0.07f;
    [SerializeField] private float lookSpeed = 45f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpForce = 5f;
    [Header("Ground Check")]
    [SerializeField, Range(0, 180)] private float maxGroundAngle = 50f;
    [SerializeField] private float groundCheckDistance = .01f;
    [SerializeField,Range(0,1)] private float groundCheckSensitivity = 0.95f;
    [SerializeField] private LayerMask groundCheckMask = Physics.AllLayers;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _jumpInput;
    private bool _sprintInput;

    private bool _isGrounded;
    private Vector3 _groundNormal;

    private void OnWalk(InputValue input) => _moveInput = input.Get<Vector2>();
    private void OnLook(InputValue input) => _lookInput = input.Get<Vector2>();
    private void OnJump(InputValue input) => _jumpInput = true;
    private void OnSprint(InputValue input) => _sprintInput = input.isPressed;

    private void FixedUpdate()
    {
        // Ground check
        _isGrounded = false;
        _groundNormal = Vector3.up;
        if (Physics.SphereCast(
            transform.TransformPoint(col.center + Vector3.down * (col.height * 0.5f - col.radius)),
            col.radius * groundCheckSensitivity,
            -transform.up,
            out RaycastHit hitInfo,
            1f - groundCheckSensitivity + groundCheckDistance,
            groundCheckMask,
            QueryTriggerInteraction.Ignore
            ))
        {
            if (Vector3.Angle(hitInfo.normal, Vector3.up) <= maxGroundAngle)
            {
                _isGrounded = true;
                _groundNormal = hitInfo.normal;
            }
        }

        // Planar Movement
        if (!IsBeingControlled) return;
        Vector3 moveCalc = (transform.forward * _moveInput.y + transform.right * _moveInput.x).normalized * moveSpeed * (_sprintInput && _isGrounded && _moveInput.y > 0 ? sprintMultiplier : 1f);
        moveCalc = Vector3.ProjectOnPlane(moveCalc, _groundNormal);
        Debug.DrawRay(transform.position - Vector3.up * col.height * 0.5f, moveCalc, Color.yellow, 0.05f);
        moveCalc = Vector3.Lerp(new Vector3(rb.velocity.x, 0f, rb.velocity.z), moveCalc, _isGrounded ? 1 : moveAerialAccelLerp);
        Debug.DrawRay(transform.position - Vector3.up * col.height * 0.5f, moveCalc, Color.red, 0.05f);

        // Vertical Movement
        if (_isGrounded && _jumpInput)
            moveCalc.y = jumpForce;
        else if (!_isGrounded)
            moveCalc.y = rb.velocity.y;

        _jumpInput = false;

        rb.velocity = moveCalc;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lookCalc = _lookInput * lookSpeed * Time.deltaTime;
        rb.transform.rotation *= Quaternion.Euler(0, lookCalc.x, 0);
        lookTransform.rotation *= Quaternion.Euler(-lookCalc.y, 0, 0);
        if (Quaternion.Angle(Quaternion.Euler(Vector3.forward), lookTransform.localRotation) > 90f)
        {
            lookTransform.localRotation = Quaternion.RotateTowards(Quaternion.Euler(Vector3.forward), lookTransform.localRotation, 90f);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!col) col = GetComponent<CapsuleCollider>();
    }

    private void OnDrawGizmos()
    {
        if(col)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawSphere(Vector3.up * (col.height * 0.5f - col.radius), col.radius);
            Gizmos.DrawSphere(Vector3.zero, col.radius);
            Gizmos.DrawSphere(Vector3.down * (col.height * 0.5f - col.radius), col.radius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(col.center + Vector3.down * (col.height * 0.5f - col.radius), col.radius);
        }
    }
#endif
}
