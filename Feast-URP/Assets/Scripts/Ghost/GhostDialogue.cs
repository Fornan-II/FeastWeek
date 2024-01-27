using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostDialogue : Interactable
{
    [SerializeField] private GhostAI ghostAI;

    private void OnEnable() => ghostAI.AddResetListener(PoolReset);
    private void OnDisable() => ghostAI.RemoveResetListener(PoolReset);

    private void PoolReset()
    {
        IsInteractable = true;
    }

    public override void Interact(Pawn interacter)
    {
        if(IsInteractable)
        {
            GhostDialogueManager.Instance.ShowDialogue();
        }
        base.Interact(interacter);
        IsInteractable = false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!ghostAI) ghostAI = GetComponent<GhostAI>();
    }
#endif
}
