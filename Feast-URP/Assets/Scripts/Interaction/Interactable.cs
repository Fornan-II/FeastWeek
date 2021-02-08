using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool IsInteractable = true;

    [SerializeField] protected Util.PawnEvent OnInteract;

    public virtual void Interact(Pawn interacter)
    {
        if(IsInteractable)
            OnInteract?.Invoke(interacter);
    }
}
