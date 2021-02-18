using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    public bool PausingAllowed = false;
    [SerializeField] private FadeUI PauseMenu;

    private float _cachedTimeScale = 1f;
    private Util.CursorMode _cachedCursorMode;

    private bool _isPaused = false;
    private bool _isExitingToMainMenu = false;

    private void Awake()
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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if(!DebugMenu.Instance)
        {
            GameObject debugMenu = new GameObject("~DebugMenu");
            debugMenu.AddComponent<DebugMenu>();
        }
#endif
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

    public void ExitToMainMenu()
    {
        if (_isExitingToMainMenu) return;

        _isExitingToMainMenu = true;
        StartCoroutine(ExitToMainMenuCoroutine());
    }

    private IEnumerator ExitToMainMenuCoroutine()
    {
        PausingAllowed = false;
        ResumeGame();
        MainCamera.Effects.CrossFade(2f, true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
        _isExitingToMainMenu = false;
    }
}
