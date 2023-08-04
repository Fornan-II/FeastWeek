﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour, DefaultControls.IGlobalActions
{
    public static PauseManager Instance { get; private set; }

    public bool PausingAllowed = false;
    public bool IsPaused => _isPaused;

#pragma warning disable 0649
    [SerializeField] private FadeUI PauseInterface;
    [SerializeField] private UnityEngine.Rendering.Volume PostProcessingVolume;
    [SerializeField] private float PostProcessingFadeSpeed = 0.3f;
    [SerializeField] private MainMenu.MenuObject PauseMenu;
    [SerializeField] private MainMenu.MenuObject OptionsMenu;
    [SerializeField] private MainMenu.MenuObject ControlsMenu;

    private Coroutine _activePostProcessingFadeCoroutine;

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

        // Caching so that we maintain a reference to _currentMenu even after it gets set to next at the end of this method
        MainMenu.MenuObject _currentCached = _currentMenu;
        _currentMenu.Menu?.FadeOut(() => _currentCached.Menu.gameObject.SetActive(false));
        next.Menu.gameObject.SetActive(true);
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
        // Early exiting on this because this callback gets called after GameManager.Instance gets set to null
        if (!GameManager.Instance) return;

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
        PauseMenu.Menu.gameObject.SetActive(true);
        PauseMenu.Menu.SetVisible();
        if(GameManager.Instance.UsingGamepadControls())
        {
            PauseMenu.FirstSelected.Select();
            PauseMenu.FirstSelected.OnSelect(null);
        }
        PauseInterface.gameObject.SetActive(true);
        PauseInterface.FadeIn();
        FadePostProcessingWeight(1f, PostProcessingFadeSpeed);

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
            
            PauseMenu.Menu.gameObject.SetActive(false);
            ControlsMenu.Menu.gameObject.SetActive(false);
            OptionsMenu.Menu.gameObject.SetActive(false);
        });
        FadePostProcessingWeight(0f, PostProcessingFadeSpeed);

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

    private void FadePostProcessingWeight(float target, float speed)
    {
        if(_activePostProcessingFadeCoroutine != null) StopCoroutine(_activePostProcessingFadeCoroutine);
        _activePostProcessingFadeCoroutine = StartCoroutine(FadePostProcessingWeightCoroutine(target, speed));
    }

    private IEnumerator FadePostProcessingWeightCoroutine(float target, float speed)
    {
        if(PostProcessingVolume.weight != target)
        {
            bool targetIsGreater = target > PostProcessingVolume.weight;

            while (targetIsGreater ? PostProcessingVolume.weight < target : PostProcessingVolume.weight > target)
            {
                PostProcessingVolume.weight += speed * Time.unscaledDeltaTime * (targetIsGreater ? 1f : -1f);
                yield return null;
            }
            PostProcessingVolume.weight = target;
        }

        _activePostProcessingFadeCoroutine = null;
    }
}
