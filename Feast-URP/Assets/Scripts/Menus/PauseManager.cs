using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour, DefaultControls.IGlobalActions
{
    public static PauseManager Instance { get; private set; }

    public bool PausingAllowed = false;
    [SerializeField] private FadeUI PauseInterface;
    [SerializeField] private MainMenu.MenuObject PauseMenu;
    [SerializeField] private MainMenu.MenuObject OptionsMenu;
    [SerializeField] private MainMenu.MenuObject ControlsMenu;

    private float _cachedTimeScale = 1f;
    private Util.CursorMode _cachedCursorMode;

    private bool _isPaused = false;
    private bool _isExitingToMainMenu = false;

    // God this menu system is so jank
    // Next time make StateMachines not Monobehaviours so I can just use them where ever, and make a menu state machine.
    // Honestly even my OG menu system is so much better than this

    private MainMenu.MenuObject _currentMenu;

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

            GameManager.Instance.Controls.Global.SetCallbacks(this);
            GameManager.Instance.Controls.Global.Enable();
            GameManager.Instance.OnControlSchemeChanged += OnControlSchemeChanged;
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
        if (Instance == this)
        {
            Instance = null;
            // Early exiting because when OnDisable() gets called during application quitting,
            // errors can occur if GameManager gets destroyed before this is called
            if (!GameManager.Instance) return;
            GameManager.Instance.Controls.Global.SetCallbacks(null);
            GameManager.Instance.Controls.Global.Disable();
            GameManager.Instance.OnControlSchemeChanged -= OnControlSchemeChanged;
        }
    }

    private void GoToMenu(MainMenu.MenuObject next)
    {
        if (_currentMenu.Menu == next.Menu) return;

        _currentMenu.Menu?.FadeOut();
        next.Menu?.FadeIn();

        if(GameManager.Instance.UsingGamepadControls())
        {
            next.FirstSelected?.Select();
            next.FirstSelected?.OnSelect(null);
        }

        _currentMenu = next;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
            TogglePause();
    }

    private void OnControlSchemeChanged()
    {
        if(GameManager.Instance.UsingGamepadControls())
        {
            _currentMenu.FirstSelected?.Select();
            _currentMenu.FirstSelected?.OnSelect(null);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
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
        PauseMenu.Menu.SetVisible();
        if(GameManager.Instance.UsingGamepadControls())
        {
            PauseMenu.FirstSelected.Select();
            PauseMenu.FirstSelected.OnSelect(null);
        }
        PauseInterface.gameObject.SetActive(true);
        PauseInterface.FadeIn();

        _currentMenu = PauseMenu;

        _isPaused = true;
    }

    public void ResumeGame()
    {
        if (!_isPaused) return;

        Time.timeScale = _cachedTimeScale;
        _cachedCursorMode.Apply();
        PauseInterface.FadeOut(() =>
        {
            PauseInterface.gameObject.SetActive(false);
            ControlsMenu.Menu.SetClear();
            OptionsMenu.Menu.SetClear();
        });

        _currentMenu = MainMenu.MenuObject.Empty;

        _isPaused = false;
    }

    public void ExitToMainMenu()
    {
        if (_isExitingToMainMenu) return;

        _isExitingToMainMenu = true;
        StartCoroutine(ExitToMainMenuCoroutine());
    }

    public void GoToOptions() => GoToMenu(OptionsMenu);
    public void GoToMain() => GoToMenu(PauseMenu);
    public void GoToControls() => GoToMenu(ControlsMenu);

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
