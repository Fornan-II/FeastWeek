using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSequence : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Interactable myInteractable;
    [SerializeField] private UnityEngine.Playables.PlayableDirector director;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private MeshRenderer ghostBones;
    [SerializeField] private GameObject deathBlob;
    [SerializeField] private AudioClip deathBlobMusic;
    [SerializeField] private AnimationCurve ghostBonesAnimation = AnimationCurve.Linear(0f, -1f, 3f, 4f);
    [SerializeField] private float alembicBlobStartTime = 2.75f;
    [SerializeField] private Controller playerController;
    [SerializeField] private BlobDetection blobDetection;
    [SerializeField] private ViewRequester deathView;
    [SerializeField] private Animator deathAnimation;
    [SerializeField] private DoorMechanic castleDoor;
    [SerializeField] private AnimationCurve initialScreenshakeAnimation;
    [SerializeField] private AnimationCurve onBlobDetectedAudioFilterCurve;
    [SerializeField] private float musicFadeOutTime = 2f;

    private bool _hasBeenActivated = false;
    private bool _deathBlobCollided = false;

    private void Start()
    {
        deathBlob.SetActive(false);
        ghostBones.material.SetFloat("_Extrusion", -1f);
    }

    public void Activate()
    {
        if (_hasBeenActivated) return;

        myInteractable.IsInteractable = false;
        _hasBeenActivated = true;
        StartCoroutine(Anim());
    }

    public void OnBlobCollision() => _deathBlobCollided = true;

    private IEnumerator RunScreenShakeCoroutine(AnimationCurve curve)
    {
        System.Action<float> screenshake = MainCamera.Effects.ContinuousScreenShake(Mathf.Max(Mathf.Epsilon, curve.Evaluate(0f)));

        for(float timer = 0.0f; timer < Util.AnimationCurveLengthTime(curve); timer += Time.deltaTime)
        {
            yield return null;
            screenshake(Mathf.Max(Mathf.Epsilon, curve.Evaluate(timer)));
        }

        screenshake(0f);
    }

    private IEnumerator Anim()
    {
        musicManager.StopImmediately();
        musicManager.FadeInNewSong(
            deathBlobMusic,
            Mathf.Max(2f, alembicBlobStartTime - 2f),
            true
        );

        // Just disable pausing during end sequence so the music is timed well enough
        PauseManager.Instance.PausingAllowed = false;

        StartCoroutine(RunScreenShakeCoroutine(initialScreenshakeAnimation));

        bool directorStarted = false;
        float animLength = Util.AnimationCurveLengthTime(ghostBonesAnimation);
        for (float timer = 0.0f; timer < animLength && !(_deathBlobCollided); timer += Time.deltaTime)
        {
            ghostBones.material.SetFloat("_Extrusion", ghostBonesAnimation.Evaluate(timer));
            if(timer >= alembicBlobStartTime && !directorStarted)
            {
                deathBlob.SetActive(true);
                director.Play();
                blobDetection.gameObject.SetActive(true);
                blobDetection.StartDetection();
            }
            yield return null;
        }

        // OR together any other bools for waiting
        while (!_deathBlobCollided) yield return null;

        // Check which of the changed bools triggered and what ending to show.
        if(_deathBlobCollided)
        {
            MainCamera.Effects.SetFadeColor(Color.black);
            MainCamera.Effects.CrossFade(0f, true);
            playerController.ReleaseControl();
            director.Stop();
            deathBlob.SetActive(false);
            castleDoor.CloseDoor();
            yield return MusicCustomFadeOut();

#if true
            deathAnimation.SetTrigger("PlayAnimation");
            deathView.RequestView();
            MainCamera.Effects.SetColorInvert(true);
            MainCamera.Effects.CrossFade(5f, false);

            yield return new WaitForSeconds(12f);
            MainCamera.Effects.CrossFade(5f, true);

            yield return new WaitForSeconds(6f);
            GlobalData.HasCompletedGame = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
#else
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
            PauseManager.Instance.PausingAllowed = true;            // Re-enabling pausing for Dark Forest scene. May remove later depending on how long sequence is.
#endif
        }
    }

    private IEnumerator MusicCustomFadeOut()
    {
        musicManager.FadeOutAnySongs(musicFadeOutTime);
        AudioLowPassFilter filter = MainCamera.Camera.gameObject.AddComponent<AudioLowPassFilter>();
        filter.lowpassResonanceQ = 1.25f;

        float animLength = Util.AnimationCurveLengthTime(onBlobDetectedAudioFilterCurve);
        for (float timer = 0.0f; timer < animLength; timer += Time.deltaTime)
        {
            filter.cutoffFrequency = onBlobDetectedAudioFilterCurve.Evaluate(timer);
            yield return null;
        }

        Destroy(filter);
    }

#if UNITY_EDITOR
    [ContextMenu("Test custom music fade out")]
    private void DebugPlayCustomMusicFadeOut()
    {
        StartCoroutine(MusicCustomFadeOut());
    }
#endif
}
