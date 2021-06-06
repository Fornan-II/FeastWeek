using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : StateMachine
{
#pragma warning disable 0649
    [System.Serializable]
    public struct MenuObject
    {
        public FadeUI Menu;
        public Button FirstSelected;

        public readonly static MenuObject Empty = new MenuObject() { Menu = null, FirstSelected = null };
    }

    [Header("Menus")]
    [SerializeField] private MenuObject main;
    [SerializeField] private MenuObject options;
    [SerializeField] private MenuObject controls;
    [Header("Game Start")]
    [SerializeField] private float fadeOutTime = 1f;
    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private int levelBuildIndex = 1;

    private MenuObject _activeMenu;

    bool _loadGameStarted = false;

    private IEnumerator Start()
    {
        Util.CursorMode.Default.Apply();
        PauseManager.Instance.PausingAllowed = false;
        MainCamera.Effects.CrossFade(1f, false);

        yield return new WaitForSeconds(1f);
        MainCamera.Effects.ResetFadeColorToDefault();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnControlSchemeChanged += OnControlSchemeChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (!GameManager.Instance) return;
        GameManager.Instance.OnControlSchemeChanged -= OnControlSchemeChanged;
    }

    private void OnControlSchemeChanged()
    {
        if (GameManager.Instance.UsingGamepadControls())
        {
            _activeMenu.FirstSelected?.Select();
            _activeMenu.FirstSelected?.OnSelect(null);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    protected override void RecalculateState()// => _activeState = StartCoroutine(_menuIsMain ? Main() : Options());
    {
        if (!_activeMenu.Menu)
            _activeMenu = main;

        _activeState = StartCoroutine(ShowMenu());
    }

    private IEnumerator ShowMenu()
    {
        MenuObject menu = _activeMenu;

        menu.Menu.gameObject.SetActive(true);
        menu.Menu.FadeIn();
        if (GameManager.Instance.UsingGamepadControls())
        {
            menu.FirstSelected.Select();
            menu.FirstSelected.OnSelect(null);
        }
        yield return new WaitUntil(() => menu.Menu != _activeMenu.Menu);

        menu.Menu.FadeOut();
        yield return new WaitUntil(() => !menu.Menu.IsFading);

        menu.Menu.gameObject.SetActive(false);
        _activeState = null;
    }

    public void SetMenuMain() => _activeMenu = main;
    public void SetMenuOptions() => _activeMenu = options;
    public void SetMenuControls() => _activeMenu = controls;
    public void QuitGame() => Application.Quit();

    public void StartGame()
    {
        // We don't want this process to be started more than once
        if (_loadGameStarted) return;
        _loadGameStarted = true;

        _activeMenu.Menu?.FadeOut();

        IEnumerator LoadGame()
        {
            // Fade Out
            for (float timer = 0.0f; timer < fadeInTime; timer += Time.deltaTime)
            {
                float tValue = Mathf.Lerp(0, 1, timer / fadeInTime);
                Shader.SetGlobalFloat("_ScreenFade", tValue * tValue);
                yield return null;
            }
            Shader.SetGlobalFloat("_ScreenFade", 1);

            // Load Level
            AsyncOperation loadOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(levelBuildIndex);
            yield return new WaitUntil(() => loadOp.isDone);

            // Fade In
            for (float timer = 0.0f; timer < fadeOutTime; timer += Time.deltaTime)
            {
                float tValue = Mathf.Lerp(1, 0, timer / fadeOutTime);
                Shader.SetGlobalFloat("_ScreenFade", tValue * tValue);
                yield return null;
            }
            Shader.SetGlobalFloat("_ScreenFade", 0);
        };

        PauseManager.Instance.StartCoroutine(LoadGame());
    }
}
