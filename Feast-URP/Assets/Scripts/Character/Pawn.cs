using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Pawn : MonoBehaviour
{
    public Controller MyController { get; protected set; }
    public bool IsBeingControlled => MyController;

    //[SerializeField] PlayerInput playerInput;
    [SerializeField] protected Util.CursorMode cursorSettings = Util.CursorMode.Default;
    [SerializeField] protected UnityEvent OnBecomeControlled;
    [SerializeField] protected UnityEvent OnStopBeingControlled;
    
    public virtual UnityAction BecomeControlledBy(Controller controller)
    {
        MyController = controller;
        ActivateInput();
        cursorSettings.Apply();
        OnBecomeControlled.Invoke();
        return StopBeingControlled;
    }

    protected virtual void StopBeingControlled()
    {
        MyController = null;
        DeactivateInput();
        OnStopBeingControlled.Invoke();
    }

    protected virtual void OnEnable()
    {
        if (IsBeingControlled)
            ActivateInput();
    }

    protected virtual void OnDisable()
    {
        // Early exiting because when OnDisable() gets called during application quitting,
        // errors can occur if GameManager gets destroyed before this is called
        if (!GameManager.Instance) return;

        DeactivateInput();
    }

    protected virtual void ActivateInput()
    {
        // Example of how to initialize 
        // GameManager.Controls.FPSCharacter.SetCallbacks(this);
        Debug.LogWarning("Pawn.ActivateInput() called! Make sure to override this method!");
    }

    protected virtual void DeactivateInput()
    {
        Debug.LogWarning("Pawn.DeactivateInput() called! Make sure to override this method!");
    }
}
