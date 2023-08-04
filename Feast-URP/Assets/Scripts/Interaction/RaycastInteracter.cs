using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInteracter : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private LayerMask interactMask = Physics.AllLayers;
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private UnityEngine.UI.Image interactIcon;
    [SerializeField] private Sprite interactSprite;
    [SerializeField] private Sprite listenSprite;

    public bool CanInteract => _targetedInteractable;

    private Interactable _targetedInteractable;
    private MsgBox _toolTipInstance;

    public void TryInteract(Pawn interacter) => _targetedInteractable?.Interact(interacter);

    private void Start()
    {
        _toolTipInstance = MsgBox.GetInstance(MsgBox.MsgBoxType.ToolTip);
    }

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
        {
            _toolTipInstance.HideMessage();

            if(interactIcon)
                interactIcon.enabled = false;
        }
        else if (!_hadInteractable && _targetedInteractable)
        {
            // Detection of interactable type that gets Tooltip message to show, and shows relevant interact icon

            if (_targetedInteractable.GetType() == typeof(GhostDialogue))
            {
                _toolTipInstance.ShowMessage(GameManager.Instance.UsingGamepadControls()
                    ? "X to listen"
                    : "Left Mouse to listen"
                    , -1f);

                if (interactIcon)
                  interactIcon.sprite = listenSprite;
            }
            else
            {
                _toolTipInstance.ShowMessage(GameManager.Instance.UsingGamepadControls()
                    ? "X to interact"
                    : "Left Mouse to interact"
                    , -1f);

                if (interactIcon)
                  interactIcon.sprite = interactSprite;
            }
            
            if(interactIcon)
                interactIcon.enabled = true;
        }
    }

    private void OnDisable()
    {
        if(_targetedInteractable)
        {
            _toolTipInstance.HideMessage();
            if(interactIcon)
                interactIcon.enabled = false;
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
