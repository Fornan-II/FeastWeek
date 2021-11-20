using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorVisibilityManager : MonoBehaviour
{
    #region Singleton Pattern
    private static CursorVisibilityManager _Instance;

    private void Awake()
    {
        if(_Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(_Instance == this)
        {
            _Instance = null;
        }
    }
    #endregion

    [SerializeField] private float timeBeforeInvisibleCursor = 10f;

    private float _timeSinceLastMouseMovement = 0.0f;

    public static bool CursorVisibility = true;
    
    private void Update()
    {
        bool cursorVisible = CursorVisibility;

        if(GameManager.Instance.UsingGamepadControls())
        {
            cursorVisible = false;
        }
        else
        {
            if(Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0f)
            {
                _timeSinceLastMouseMovement = 0f;
            }
            else
            {
                _timeSinceLastMouseMovement += Time.unscaledDeltaTime;
            }

            if(_timeSinceLastMouseMovement > timeBeforeInvisibleCursor)
            {
                cursorVisible = false;
            }
        }

        Cursor.visible = cursorVisible;
    }
}
