using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWorld : MonoBehaviour
{
    [SerializeField] private MusicManager musicManagerRef;
    [Header("Intro")]
    [SerializeField] private float cameraFadeInDuration = 7f;
    [SerializeField] private float musicFadeInDuration = 1f;
    [SerializeField] private AudioClip primaryAmbientMusic;
    [Header("End sequence general")]
    [SerializeField] private LookAtTarget godLookAtTarget;
    [SerializeField] private AudioClip endSequenceMusic;
    [Header("End sequence part 1")]
    [SerializeField] private FPSChar playerCharacter;
    [SerializeField] private Transform forestGodFaceTransform;
    [SerializeField] private CameraView endSequence1View;
    [SerializeField] private float maxCameraVelocity = 2.5f;
    [SerializeField] private float cameraSmoothing = 1f;
    [SerializeField] private AnimationCurve lookAtAnim;
    [Header("End sequence part 2")]
    [SerializeField] private CameraView endSequence2View;
    [SerializeField] private RandomBlink godBlink;
    [SerializeField] private AnimationCurve eyeOpenAnim;
    [SerializeField] private AnimationCurve eyeCloseAnim;
    [Header("End sequence part 3")]
    [SerializeField] private CameraView endSequence3View;
    [SerializeField] private GameObject forestGeometry;
    [SerializeField] private GameObject hallGeometry;
    [SerializeField] private Animator endSequence3Animator;
    [Header("End sequence part 4")]
    [SerializeField] private float finalFadeOutDuration = 10f;
    [SerializeField, ColorUsage(false, true)] private Color fadeColor;
    [SerializeField] private float blackOutLingerTime = 7f;

    //bool _endAnimStarted = false;
    //bool _endAnimReadyToEnd = false;
    int _endSequenceProgress = -1;

    private void Start()
    {
        MainCamera.Effects.SetFadeColor(Color.black);
        MainCamera.Effects.CrossFade(cameraFadeInDuration, false);
        musicManagerRef.FadeInNewSong(primaryAmbientMusic, musicFadeInDuration, true);
    }

    public void StartEndAnimation()
    {
        if (_endSequenceProgress >= 0) return;
            _endSequenceProgress = 0;
        StartCoroutine(EndAnimation());
    }

    public void EndEndAnimation() => _endSequenceProgress = 3;

    private IEnumerator EndAnimation()
    {
        // End sequence part 1
        if(PauseManager.Instance)
            PauseManager.Instance.PausingAllowed = false;
        playerCharacter.MyController.ReleaseControl();

        Util.MoveTransformToTarget(endSequence1View.transform, MainCamera.RootTransform);
        endSequence1View.RequestView();

        godLookAtTarget.Target = endSequence1View.transform;

        Vector3 camVelocity = playerCharacter.GetVelocity();
        Vector3 dampVelocity = Vector3.zero;

        Quaternion initRotation = MainCamera.RootTransform.rotation;

        float timer = 0.0f;
        PlayEndMusic();
        
        while (_endSequenceProgress < 1)
        {
            Vector3 targetPos = forestGodFaceTransform.position;
            targetPos.y = playerCharacter.LookTransform.position.y;
            Vector3 targetVel = (targetPos - endSequence1View.transform.position).normalized * maxCameraVelocity;

            camVelocity = Vector3.SmoothDamp(camVelocity, targetVel, ref dampVelocity, cameraSmoothing);
            endSequence1View.transform.position += camVelocity * Time.deltaTime;

            Vector3 vecToTarget = forestGodFaceTransform.position - endSequence1View.transform.position;
            endSequence1View.transform.rotation = Quaternion.Slerp(initRotation, Quaternion.LookRotation(vecToTarget), lookAtAnim.Evaluate(timer));

            yield return null;
            timer += Time.deltaTime;
        }

        // End sequence part 2
        endSequence2View.RequestView();
        godBlink.enabled = false;
        godBlink.SetOpenness(1f);

        //yield return new WaitUntil(() => _endSequenceProgress >= 2);
        // Temp alternative to give forest god just a little bit of secondary motion until I get a proper animation for them
        Transform target = new GameObject("~LookAtTarget").transform;
        target.position = godLookAtTarget.Target.position;
        godLookAtTarget.Target = target;
        float targetSpeed = 1f;
        while (_endSequenceProgress < 2)
        {
            target.position = Vector3.MoveTowards(
                target.position,
                Vector3.zero,
                targetSpeed * Time.deltaTime
                );
            yield return null;
        }

        // Close god's eyes
        for (timer = 0.0f; timer < Util.AnimationCurveLengthTime(eyeCloseAnim); timer += Time.deltaTime)
        {
            float t = eyeCloseAnim.Evaluate(timer);
            godBlink.SetOpenness(t);
            MainCamera.Effects.ManuallySetScreenFade(1f - t);

            // Also temp
            target.position = Vector3.MoveTowards(
                target.position,
                Vector3.zero,
                targetSpeed * Time.deltaTime
                );

            yield return null;
        }

        // End sequence part 3
        forestGeometry.SetActive(false);
        hallGeometry.SetActive(true);

        endSequence3View.RequestView();
        godLookAtTarget.Target = endSequence3View.transform;
        endSequence3Animator.SetTrigger("EndSequence3");

        godBlink.SetOpenness(1f);
        godBlink.enabled = true;

        // Default MainCamera clip planes are different too allow player to get really close to things
        // Setting it to a farther distance for cutscene.
        MainCamera.Camera.nearClipPlane = 0.3f;
        MainCamera.Camera.farClipPlane = 1000f;
        MainCamera.Effects.ManuallySetScreenFade(0f);

        yield return new WaitUntil(() => _endSequenceProgress >= 3);

        // End sequence part 4
        MainCamera.Effects.SetFadeColor(fadeColor);
        MainCamera.Effects.CrossFade(finalFadeOutDuration, true);
        yield return new WaitForSeconds(finalFadeOutDuration + blackOutLingerTime);
        // Fade out audio?
        if (PauseManager.Instance)
            PauseManager.Instance.PausingAllowed = true;
        GlobalData.HasCompletedGame = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void PlayEndMusic()
    {
        Song endMusic = musicManagerRef.CrossfadeInNewSong(endSequenceMusic, 6.89f);

        // Scripted song events
        endMusic.AddSongEvent(6.858f, () => _endSequenceProgress = 1);
        endMusic.AddSongEvent(12.479f - 0.75f, () => _endSequenceProgress = 2);
    }
}
