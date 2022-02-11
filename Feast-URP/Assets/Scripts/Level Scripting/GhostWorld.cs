using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWorld : MonoBehaviour
{
    [SerializeField] private float fadeInDuration = 7f;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float delayBeforeReturnToMenu = 5f;

    private void Start()
    {
        MainCamera.Effects.SetFadeColor(Color.black);
        MainCamera.Effects.CrossFade(fadeInDuration, false);
    }

    private void OnAnimComplete()
    {
        MainCamera.Effects.SetFadeColor(Color.white);
        MainCamera.Effects.CrossFade(delayBeforeReturnToMenu, true);
        StartCoroutine(Util.FadeMusicSource(musicSource, delayBeforeReturnToMenu, 0f));
        GlobalData.HasCompletedGame = true;
        StartCoroutine(WaitThenReturnToMainMenu());
    }

    private IEnumerator WaitThenReturnToMainMenu()
    {
        yield return new WaitForSeconds(delayBeforeReturnToMenu);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
