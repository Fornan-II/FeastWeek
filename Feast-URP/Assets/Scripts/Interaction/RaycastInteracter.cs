using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInteracter : MonoBehaviour
{
    [SerializeField] private LayerMask interactMask = Physics.AllLayers;
    [SerializeField] private float interactRange = 1.5f;

    public bool CanInteract => _targetedInteractable;

    private Interactable _targetedInteractable;

    public void TryInteract(Pawn interacter) => _targetedInteractable?.Interact(interacter);

    private void FixedUpdate()
    {
        _targetedInteractable = null;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, interactRange, interactMask, QueryTriggerInteraction.Ignore))
        {
            hitInfo.transform.TryGetComponent(out _targetedInteractable);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = CanInteract ? Color.yellow : Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * interactRange);
    }
#endif
}
