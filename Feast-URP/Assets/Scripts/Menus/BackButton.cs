using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private void OnEnable()
    {
        InputSystemUIInputModule inputSystem = UnityEngine.EventSystems.EventSystem.current?.GetComponent<InputSystemUIInputModule>();

        if(inputSystem)
            inputSystem.cancel.action.started += OnCancelStarted;
        else
            Debug.LogWarning("[BackButton] Attempted to register actions to current EventSystem but couldn't find one.");
    }

    private void OnDisable()
    {
        InputSystemUIInputModule inputSystem = UnityEngine.EventSystems.EventSystem.current?.GetComponent<InputSystemUIInputModule>();

        if (inputSystem)
            inputSystem.cancel.action.started -= OnCancelStarted;
        else
            Debug.LogWarning("[BackButton] Attempted to register actions to current EventSystem but couldn't find one.");
    }

    private void OnCancelStarted(InputAction.CallbackContext obj)
    {
        backButton.onClick.Invoke();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!backButton) backButton = GetComponent<Button>();
    }
#endif
}
