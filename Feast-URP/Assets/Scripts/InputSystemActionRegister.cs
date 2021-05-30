using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class InputSystemActionRegister : MonoBehaviour
{
    [SerializeField] private InputSystemUIInputModule inputSystem;

    private void OnEnable()
    {
        foreach(var map  in inputSystem.actionsAsset.actionMaps)
        {
            GameManager.Instance.RegisterActions(map);
        }
    }

    private void OnDisable()
    {
        if (!GameManager.Instance) return;

        foreach (var map in inputSystem.actionsAsset.actionMaps)
        {
            GameManager.Instance.UnregisterActions(map);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!inputSystem) inputSystem = GetComponent<InputSystemUIInputModule>();
    }
#endif
}
