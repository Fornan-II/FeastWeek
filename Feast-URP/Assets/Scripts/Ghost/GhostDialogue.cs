using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostDialogue : Interactable
{
    public override void Interact(Pawn interacter)
    {
        if(IsInteractable)
        {
            GhostDialogueManager.Instance.ShowDialogue();
        }
        base.Interact(interacter);
        IsInteractable = false;
    }
}
