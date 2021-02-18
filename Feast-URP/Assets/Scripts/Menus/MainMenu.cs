using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : StateMachine
{
    [Header("Menus")]
    [SerializeField] private FadeUI main;
    [SerializeField] private FadeUI options;
    [Header("Game Start")]
    [SerializeField] private float fadeOutTime = 1f;
    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private int levelBuildIndex;

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
        main.gameObject.SetActive(true);
        main.FadeIn();
        yield return new WaitUntil(() => !_menuIsMain && !main.IsFading);
        main.FadeOut();
        yield return new WaitUntil(() => !main.IsFading);
        main.gameObject.SetActive(false);
        _activeState = null;
    }

    private IEnumerator Options()
    {
        options.gameObject.SetActive(true);
        options.FadeIn();
        yield return new WaitUntil(() => _menuIsMain && !options.IsFading);
        options.FadeOut();
        yield return new WaitUntil(() => !options.IsFading);
        options.gameObject.SetActive(false);
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
            main.FadeOut();
        else
            options.FadeOut();

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
