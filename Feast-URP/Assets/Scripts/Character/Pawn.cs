using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Pawn : MonoBehaviour
{
    public Controller MyController { get; protected set; }
    public bool IsBeingControlled => MyController;

    [SerializeField] PlayerInput playerInput;
    [SerializeField] protected Util.CursorMode cursorSettings = Util.CursorMode.Default;
    [SerializeField] protected UnityEvent OnBecomeControlled;
    [SerializeField] protected UnityEvent OnStopBeingControlled;

    public UnityAction BecomeControlledBy(Controller controller)
    {
        MyController = controller;
        playerInput.enabled = true;
        cursorSettings.Apply();
        OnBecomeControlled.Invoke();
        return StopBeingControlled;
    }

    private void StopBeingControlled()
    {
        MyController = null;
        playerInput.enabled = false;
        OnStopBeingControlled.Invoke();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(!playerInput && TryGetComponent(out playerInput))
        {
            playerInput.DeactivateInput();
        }
    }
#endif
}
