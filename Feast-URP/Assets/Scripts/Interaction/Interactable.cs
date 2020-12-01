using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected Util.PawnEvent OnInteract;

    public virtual void Interact(Pawn interacter) => OnInteract?.Invoke(interacter);
}
