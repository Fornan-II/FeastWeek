using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    [SerializeField] private InputSystemUIInputModule inputSystem;
    [SerializeField] private Button backButton;

    private void OnEnable()
    {
        inputSystem.cancel.action.started += OnCancelStarted;
    }

    private void OnDisable()
    {
        if(inputSystem)
            inputSystem.cancel.action.started -= OnCancelStarted;
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
