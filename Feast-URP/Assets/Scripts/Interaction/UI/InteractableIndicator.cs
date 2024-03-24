using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableIndicator : MonoBehaviour
{
    [SerializeField] private Image icon;

    private Interactable _trackedInteractable;

    public void SetInteractable(Interactable interactable) => _trackedInteractable = interactable;

    private void Update()
    {
        if(!_trackedInteractable)
        {
            icon.enabled = false;
            return;
        }

        icon.enabled = true;
        
        Vector3 screenPos = MainCamera.Camera.WorldToScreenPoint(_trackedInteractable.transform.position);
        icon.rectTransform.position = screenPos;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!icon) icon = GetComponent<Image>();
    }
#endif
}
