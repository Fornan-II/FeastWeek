using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWorld : MonoBehaviour
{
    public bool IsEndSequenceInProgress() => _endSequenceProgress >= 0;

#pragma warning disable 0649
    [SerializeField] private MusicManager musicManagerRef;
    [SerializeField] private GhostWorldHelper ghostWorldHelper;
    [SerializeField] private float cameraNoisePulseStrength = 0;
    [SerializeField] private float cameraNoisePulseSpeed = 1;
    [SerializeField] private float cameraNoisePulseExponent = 1;
    [Header("Intro")]
    [SerializeField] private float cameraFadeInDuration = 7f;
    [SerializeField] private float musicFadeInDuration = 1f;
    [SerializeField] private AudioClip primaryAmbientMusic;
    [Header("End sequence general")]
    [SerializeField] private ForestGodAI forestGodAI;
    [SerializeField] private LookAtTarget godLookAtTarget;
    [SerializeField] private AudioClip endSequenceMusic;
    [Header("End sequence part 1")]
    [SerializeField] private FPSChar playerCharacter;
    [SerializeField] private Transform forestGodFaceTransform;
    [SerializeField] private CameraView endSequence1View;
    [SerializeField] private float maxCameraVelocity = 2.5f;
    [SerializeField] private float cameraSmoothing = 1f;
    [SerializeField] private AnimationCurve lookAtAnim;
    [SerializeField] private AnimationCurve noiseAnimPart1;
    [Header("End sequence part 2")]
    [SerializeField] private CameraView endSequence2View;
    [SerializeField] private RandomBlink godBlink;
    [SerializeField] private AnimationCurve eyeOpenAnim;
    [SerializeField] private AnimationCurve eyeCloseAnim;
    [SerializeField] private AnimationCurve noiseAnimPart2;
    [Header("End sequence part 3")]
    [SerializeField] private CameraView endSequence3View;
    [SerializeField] private GameObject forestGeometry;
    [SerializeField] private GameObject hallGeometry;
    [SerializeField] private Transform forestGodFinalPosition;
    [SerializeField] private Animator endSequence3Animator;
    [Header("End sequence part 4")]
    [SerializeField] private float finalFadeOutDuration = 10f;
    [SerializeField, ColorUsage(false, true)] private Color fadeColor;
    [SerializeField] private float blackOutLingerTime = 7f;
    
    int _endSequenceProgress = -1;

    private void Start()
    {
        MainCamera.Effects.SetFadeColor(Color.black);
        MainCamera.Effects.CrossFade(cameraFadeInDuration, false);
        musicManagerRef.FadeInNewSong(primaryAmbientMusic, musicFadeInDuration, true);
        SetCameraPulseSettings();
    }

    private void SetCameraPulseSettings()
    {
        MainCamera.Effects.ApplyCameraNoisePulse(GetInstanceID(), cameraNoisePulseStrength, cameraNoisePulseSpeed, cameraNoisePulseExponent);
    }

    private void OnDestroy()
    {
        MainCamera.Effects?.RemoveCameraNoisePulse(GetInstanceID());
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
        // Player control is relinquished, character staring at and walking towards forest god.
        forestGodAI.TriggerPlayerAggro();

        if (PauseManager.Instance)
            PauseManager.Instance.PausingAllowed = false;
        playerCharacter.MyController.ReleaseControl();

        Util.MoveTransformToTarget(endSequence1View.transform, MainCamera.RootTransform);
        endSequence1View.RequestView();

        godLookAtTarget.Target = endSequence1View.transform;

        Vector3 camVelocity = playerCharacter.GetVelocity();
        Vector3 dampVelocity = Vector3.zero;

        Quaternion initRotation = MainCamera.RootTransform.rotation;

        MainCamera.Effects.RemoveCameraNoisePulse(GetInstanceID());

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

            MainCamera.Effects.ApplyCameraNoise(GetInstanceID(), noiseAnimPart1.Evaluate(timer));

            yield return null;
            timer += Time.deltaTime;
        }

        // End sequence part 2
        // Close-up of forest god's face
        endSequence2View.RequestView();
        godLookAtTarget.Target = endSequence2View.transform;
        godBlink.enabled = false;
        godBlink.SetOpenness(1f);
        float noiseTimer = 0.0f;

        while (_endSequenceProgress < 2)
        {
            MainCamera.Effects.ApplyCameraNoise(GetInstanceID(), noiseAnimPart2.Evaluate(noiseTimer));
            yield return null;
            noiseTimer += Time.deltaTime;
        }

        // Close god's eyes
        for (timer = 0.0f; timer < Util.AnimationCurveLengthTime(eyeCloseAnim); timer += Time.deltaTime)
        {
            float t = eyeCloseAnim.Evaluate(timer);
            godBlink.SetOpenness(t);
            MainCamera.Effects.ManuallySetScreenFade(1f - t);

            MainCamera.Effects.ApplyCameraNoise(GetInstanceID(), noiseAnimPart2.Evaluate(noiseTimer));
            
            yield return null;
            noiseTimer += Time.deltaTime;
        }

        // End sequence part 3
        // Camera beginning to dolly back, revealing castle interior
        forestGeometry.SetActive(false);
        hallGeometry.SetActive(true);

        Util.MoveTransformToTarget(forestGodAI.transform, forestGodFinalPosition);
        forestGodAI.SetState(ForestGodAI.ForestGodState.SIT, true);

        endSequence3View.RequestView();
        godLookAtTarget.Target = endSequence3View.transform;
        endSequence3Animator.SetTrigger("EndSequence3");

        godBlink.SetOpenness(1f);
        godBlink.enabled = true;

        // Default MainCamera clip planes are different to allow player to get really close to things
        // Setting it to a farther distance for cutscene.
        MainCamera.Camera.nearClipPlane = 0.3f;
        MainCamera.Camera.farClipPlane = 1000f;
        MainCamera.Effects.ManuallySetScreenFade(0f);

        while(_endSequenceProgress < 3)
        {
            MainCamera.Effects.ApplyCameraNoise(GetInstanceID(), ghostWorldHelper.NoiseStrengthProxy);
            yield return null;
        }

        // End sequence part 4
        // Final fade to black
        MainCamera.Effects.SetFadeColor(fadeColor);
        MainCamera.Effects.CrossFade(finalFadeOutDuration, true);
        for(timer = 0.0f; timer < finalFadeOutDuration + blackOutLingerTime; timer += Time.deltaTime)
        {
            MainCamera.Effects.ApplyCameraNoise(GetInstanceID(), ghostWorldHelper.NoiseStrengthProxy);
            yield return null;
        }
        // Fade out audio?
        if (PauseManager.Instance)
            PauseManager.Instance.PausingAllowed = true;
        GameManager.Instance.SetGameCompleted();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void PlayEndMusic()
    {
        Song endMusic = musicManagerRef.CrossfadeInNewSong(endSequenceMusic, 6.89f);

        // Scripted song events
        endMusic.AddSongEvent(6.858f, () => _endSequenceProgress = 1);
        endMusic.AddSongEvent(12.479f - 0.75f, () => _endSequenceProgress = 2);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!UnityEditor.EditorApplication.isPlaying || _endSequenceProgress >= 0) return;

        if(MainCamera.IsValid())
            SetCameraPulseSettings();
    }
#endif
}
