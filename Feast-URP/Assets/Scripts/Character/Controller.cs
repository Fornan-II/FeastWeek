using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Controller : MonoBehaviour
{
    public Pawn ControlledPawn => controlledPawn;

    [SerializeField] protected Pawn controlledPawn;
    private UnityAction _pawnReleaseMethod;

    private void Start()
    {
        TakeControlOf(controlledPawn);
    }

    public void TakeControlOf(Pawn pawn)
    {
#if UNITY_EDITOR
        if(!UnityEditor.EditorApplication.isPlaying)
        {
            controlledPawn = pawn;
            return;
        }
#endif
        // Already controlling this pawn, don't need to do anything.
        if (pawn == controlledPawn) return;

        if (controlledPawn && _pawnReleaseMethod != null)
        {
            _pawnReleaseMethod();
            _pawnReleaseMethod = null;
        }

        controlledPawn = pawn;
#if UNITY_EDITOR
        _cachedControlledPawn = pawn;
#endif
        if(controlledPawn)
        {
            _pawnReleaseMethod = controlledPawn.BecomeControlledBy(this);
        }
    }

    public void ReleaseControl() => TakeControlOf(null);

#if UNITY_EDITOR
    private Pawn _cachedControlledPawn;
    private void OnValidate()
    {
        if(UnityEditor.EditorApplication.isPlaying)
        {
            if (controlledPawn && controlledPawn.MyController != this)
                controlledPawn.BecomeControlledBy(this);
            else if (!controlledPawn && _cachedControlledPawn && _cachedControlledPawn.MyController == this)
            {
                controlledPawn = _cachedControlledPawn;
                ReleaseControl();
            }
        }
    }
#endif
}
