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
        bool _hadInteractable = _targetedInteractable;

        _targetedInteractable = null;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, interactRange, interactMask, QueryTriggerInteraction.Ignore))
        {
            if(hitInfo.transform.TryGetComponent(out _targetedInteractable))
            {
                _targetedInteractable = _targetedInteractable.IsInteractable ? _targetedInteractable : null;
            }
        }

        if (_hadInteractable && !_targetedInteractable)
            MsgBox.HideMessage();
        else if (!_hadInteractable && _targetedInteractable)
            MsgBox.ShowMessage("Left Mouse to Interact", -1f);
    }

    private void OnDisable()
    {
        if(_targetedInteractable)
        {
            MsgBox.HideMessage();
            _targetedInteractable = null;
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
