using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public static bool PausingAllowed = false;

    [SerializeField] private FadeUI PauseMenu;

    private float _cachedTimeScale = 1f;
    private Util.CursorMode _cachedCursorMode;

    private bool _isPaused = false;

    private void Start()
    {
        if(Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(Instance == this)
            Instance = null;
    }

    private void OnPause(InputValue input) => TogglePause();

    public void TogglePause()
    {
        if (_isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (_isPaused || !PausingAllowed) return;

        _cachedTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        _cachedCursorMode = Util.CursorMode.GetCurrent();
        Util.CursorMode.Default.Apply();
        if (PauseMenu)
        {
            PauseMenu.FadeIn();
            PauseMenu.gameObject.SetActive(true);
        }
        _isPaused = true;
    }

    public void ResumeGame()
    {
        if (!_isPaused) return;

        Time.timeScale = _cachedTimeScale;
        _cachedCursorMode.Apply();
        if (PauseMenu) PauseMenu.FadeOut(() => { PauseMenu.gameObject.SetActive(false); });
        _isPaused = false;
    }

    public void QuitGame() => Application.Quit();
}
