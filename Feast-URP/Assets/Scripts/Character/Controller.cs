using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controller : MonoBehaviour
{
    [SerializeField] protected Pawn controlledPawn;
    private UnityAction _pawnReleaseMethod;

    private void Start() => controlledPawn?.BecomeControlledBy(this);

    public void TakeControlOf(Pawn pawn)
    {
        if (controlledPawn)
        {
            _pawnReleaseMethod();
            _pawnReleaseMethod = null;
        }

        controlledPawn = pawn;
        if(controlledPawn)
        {
            _pawnReleaseMethod = controlledPawn.BecomeControlledBy(this);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(UnityEditor.EditorApplication.isPlaying && controlledPawn && controlledPawn.MyController != this)
        {
            controlledPawn.BecomeControlledBy(this);
        }
    }
#endif
}
