using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using System;
using UnityEngine.InputSystem;

public class UIScrollRectGamepadHelper : MonoBehaviour
{
    // Use this so only one scroll rect is active, and it's the most recently displayed one
    // Using list instead of stack to make use of .Remove()
    private static List<UIScrollRectGamepadHelper> scrollRectStack = new List<UIScrollRectGamepadHelper>();

    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private float scrollSpeedMultiplier = 1f;

    private Vector2 scrollInput = Vector2.zero;

    private void OnEnable()
    {
        scrollRectStack.Add(this);

        InputSystemUIInputModule inputSystem = UnityEngine.EventSystems.EventSystem.current?.GetComponent<InputSystemUIInputModule>();

        if (inputSystem)
        {
            inputSystem.scrollWheel.action.started += OnScroll;
            inputSystem.scrollWheel.action.performed += OnScroll;
            inputSystem.scrollWheel.action.canceled += OnScroll;
        }
        else
            Debug.LogWarning("[BackButton] Attempted to register actions to current EventSystem but couldn't find one.");
    }

    private void OnDisable()
    {
        scrollRectStack.Remove(this);

        InputSystemUIInputModule inputSystem = UnityEngine.EventSystems.EventSystem.current?.GetComponent<InputSystemUIInputModule>();

        if (inputSystem)
        {
            inputSystem.scrollWheel.action.started -= OnScroll;
            inputSystem.scrollWheel.action.performed -= OnScroll;
            inputSystem.scrollWheel.action.canceled -= OnScroll;
        }
        else
            Debug.LogWarning("[BackButton] Attempted to register actions to current EventSystem but couldn't find one.");
    }


    private void OnScroll(InputAction.CallbackContext obj) => scrollInput = obj.ReadValue<Vector2>();

    private bool IsTopOfStack() => scrollRectStack[scrollRectStack.Count - 1] == this;

    void Update()
    {
        // Only apply when using a controller
        if (IsTopOfStack() && GameManager.Instance.UsingGamepadControls())
        {
            // SUPER important to be using Time.unscaledDeltaTime...
            // I thought this script was irreparably broken because it just didn't work in the pause menu
            // ...where Time.timeScale is 0 :)
            scrollbar.value = Mathf.Clamp01(scrollbar.value + scrollSpeedMultiplier * scrollInput.y * Time.unscaledDeltaTime);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!scrollbar) scrollbar = GetComponent<Scrollbar>();
    }
#endif
}
