using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour, DefaultControls.IGlobalActions
{
    public static PauseManager Instance { get; private set; }

    public bool PausingAllowed = false;
    [SerializeField] private FadeUI PauseInterface = null;
    [SerializeField] private FadeUI PauseMenu = null;
    [SerializeField] private FadeUI OptionsMenu = null;

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

    private void OnEnable()
    {
        GameManager.Instance.Controls.Global.SetCallbacks(this);
        GameManager.Instance.Controls.Global.Enable();
    }

    private void OnDisable()
    {
        // Early exiting because when OnDisable() gets called during application quitting,
        // errors can occur if GameManager gets destroyed before this is called
        if (!GameManager.Instance) return;

        GameManager.Instance.Controls.Global.SetCallbacks(null);
        GameManager.Instance.Controls.Global.Disable();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
            TogglePause();
    }

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
        if (PauseInterface)
        {
            PauseInterface.gameObject.SetActive(true);
            PauseInterface.FadeIn();
        }
        _isPaused = true;
    }

    public void ResumeGame()
    {
        if (!_isPaused) return;

        Time.timeScale = _cachedTimeScale;
        _cachedCursorMode.Apply();
        if (PauseInterface)
        {
            PauseInterface.FadeOut(() =>
            {
                PauseInterface.gameObject.SetActive(false);
                OptionsMenu.SetClear();
                PauseMenu.SetVisible();
            });
        }
        _isPaused = false;
    }

    public void ExitToMainMenu()
    {
        if (_isExitingToMainMenu) return;

        _isExitingToMainMenu = true;
        StartCoroutine(ExitToMainMenuCoroutine());
    }

    public void GoToOptions()
    {
        if (!_isPaused) return;

        PauseMenu.FadeOut();
        OptionsMenu.FadeIn();
    }

    public void ExitOptions()
    {
        OptionsMenu.FadeOut();
        PauseMenu.FadeIn();
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
