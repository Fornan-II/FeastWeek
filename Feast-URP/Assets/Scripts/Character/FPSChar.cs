using UnityEngine;
using UnityEngine.InputSystem;

public class FPSChar : Pawn, ICheckpointUser
{
#pragma warning disable 0649
    [Header("Components")]
    [SerializeField] private CharacterController movementController;
    [SerializeField] private RaycastInteracter interacter;
    [SerializeField] private Transform lookTransform;
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
    private void OnInteract(InputValue input) => interacter.TryInteract(this);

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

    // Update is called once per frame
    void Update()
    {
        if (!IsBeingControlled || Time.timeScale <= 0f) return;

        GroundCheck();
        PlayerMovement();
        PlayerLook();
    }

    private void PlayerMovement()
    {
        if (!IsBeingControlled) return;
        // Planar Movement
        Vector3 moveCalc = (transform.forward * _moveInput.y + transform.right * _moveInput.x).normalized * moveSpeed * (_sprintInput && _isGrounded && _moveInput.y > 0 ? sprintMultiplier : 1f);
        moveCalc = Vector3.ProjectOnPlane(moveCalc, _groundNormal);
        Debug.DrawRay(transform.position - Vector3.up * movementController.height * 0.5f, moveCalc, Color.yellow, 0.05f);
        moveCalc = Vector3.Lerp(new Vector3(movementController.velocity.x, 0f, movementController.velocity.z), moveCalc, _isGrounded ? 1 : moveAerialAccelLerp);
        Debug.DrawRay(transform.position - Vector3.up * movementController.height * 0.5f, moveCalc, Color.red, 0.05f);

        // Vertical Movement
        if (_isGrounded && _jumpInput)
            moveCalc.y = jumpForce;
        else if(!_isGrounded)
            moveCalc.y = movementController.velocity.y;

        _jumpInput = false;
        moveCalc += Physics.gravity * Time.deltaTime;
        movementController.Move(moveCalc * Time.deltaTime);
    }

    private void PlayerLook()
    {
        Vector2 lookCalc = _lookInput * lookSpeed;// * Time.deltaTime; // InputSystem mouse offsets are already time-scaled!
        transform.rotation *= Quaternion.Euler(0, lookCalc.x, 0);
        lookTransform.rotation *= Quaternion.Euler(-lookCalc.y, 0, 0);
        if (Quaternion.Angle(Quaternion.Euler(Vector3.forward), lookTransform.localRotation) > 90f)
        {
            lookTransform.localRotation = Quaternion.RotateTowards(Quaternion.Euler(Vector3.forward), lookTransform.localRotation, 90f);
        }
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
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        movementController = movementController ?? GetComponent<CharacterController>();
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
