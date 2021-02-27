using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : StateMachine
{
    [System.Serializable]
    private struct MenuObject
    {
        public FadeUI Menu;
        public Button FirstSelected;
    }

#pragma warning disable 0649
    [Header("Menus")]
    [SerializeField] private MenuObject main;
    [SerializeField] private MenuObject options;
    [Header("Game Start")]
    [SerializeField] private float fadeOutTime = 1f;
    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private int levelBuildIndex = 1;

    bool _menuIsMain = true;
    bool _loadGameStarted = false;

    private IEnumerator Start()
    {
        Util.CursorMode.Default.Apply();
        PauseManager.Instance.PausingAllowed = false;
        MainCamera.Effects.CrossFade(1f, false);

        yield return new WaitForSeconds(1f);
        MainCamera.Effects.ResetFadeColorToDefault();
    }

    protected override void RecalculateState() => _activeState = StartCoroutine(_menuIsMain ? Main() : Options());

    private IEnumerator Main()
    {
        main.Menu.gameObject.SetActive(true);
        main.Menu.FadeIn();
        main.FirstSelected.Select();
        main.FirstSelected.OnSelect(null);
        yield return new WaitUntil(() => !_menuIsMain);

        main.Menu.FadeOut();
        yield return new WaitUntil(() => !main.Menu.IsFading);

        main.Menu.gameObject.SetActive(false);
        _activeState = null;
    }

    private IEnumerator Options()
    {
        options.Menu.gameObject.SetActive(true);
        options.Menu.FadeIn();
        options.FirstSelected.Select();
        options.FirstSelected.OnSelect(null);
        yield return new WaitUntil(() => _menuIsMain && !options.Menu.IsFading);

        options.Menu.FadeOut();
        yield return new WaitUntil(() => !options.Menu.IsFading);

        options.Menu.gameObject.SetActive(false);
        _activeState = null;
    }

    public void SetMenuMain() => _menuIsMain = true;
    public void SetMenuOptions() => _menuIsMain = false;
    public void QuitGame() => Application.Quit();

    public void StartGame()
    {
        // We don't want this process to be started more than once
        if (_loadGameStarted) return;
        _loadGameStarted = true;

        if (_menuIsMain)
            main.Menu.FadeOut();
        else
            options.Menu.FadeOut();

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
