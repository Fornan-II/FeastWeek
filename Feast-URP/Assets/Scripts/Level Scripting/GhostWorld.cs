using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWorld : MonoBehaviour
{
    [Header("Intro")]
    [SerializeField] private float fadeInDuration = 7f;
    [Header("End sequence general")]
    [SerializeField] private LookAtTarget godLookAtTarget;
    [SerializeField] private RandomBlink godBlink;
    [SerializeField] private AnimationCurve eyeOpenAnim;
    [SerializeField] private AnimationCurve eyeCloseAnim;
    [Header("End sequence part 1")]
    [SerializeField] private FPSChar playerCharacter;
    [SerializeField] private Transform forestGodFaceTransform;
    [SerializeField] private ViewRequester endSequence1View;
    [SerializeField] private float maxCameraVelocity = 2.5f;
    [SerializeField] private float part1EndRadius = 10f;
    [SerializeField] private float cameraSmoothing = 1f;
    [SerializeField] private AnimationCurve lookAtAnim;
    [Header("End sequence part 2")]
    [SerializeField] private ViewRequester endSequence2View;
    [SerializeField] private float part2Duration = 0.5f;
    [Header("End sequence part 3")]
    [SerializeField] private ViewRequester endSequence3View;
    [SerializeField] private GameObject forestGeometry;
    [SerializeField] private GameObject hallGeometry;
    [SerializeField] private Animator endSequence3Animator;
    [Header("End sequence part 4")]
    [SerializeField] private float finalFadeOutDuration = 10f;
    [SerializeField] private float blackOutLingerTime = 7f;

    bool _endAnimStarted = false;
    bool _endAnimReadyToEnd = false;

    private void Start()
    {
        MainCamera.Effects.SetFadeColor(Color.black);
        MainCamera.Effects.CrossFade(fadeInDuration, false);
    }

    public void StartEndAnimation()
    {
        if (_endAnimStarted) return;
        _endAnimStarted = true;
        StartCoroutine(EndAnimation());
    }

    public void EndEndAnimation() => _endAnimReadyToEnd = true;

    private IEnumerator EndAnimation()
    {
        // End sequence part 1
        PauseManager.Instance.PausingAllowed = false;
        playerCharacter.MyController.ReleaseControl();

        Util.MoveTransformToTarget(endSequence1View.transform, MainCamera.RootTransform);
        endSequence1View.RequestView();

        godLookAtTarget.Target = endSequence1View.transform;

        Vector3 camVelocity = playerCharacter.GetVelocity();
        Vector3 dampVelocity = Vector3.zero;

        Quaternion initRotation = MainCamera.RootTransform.rotation;

        bool fadeStarted = false;
        bool continueCameraMove = true;
        float timer = 0.0f;
        float fadeStartTime = Mathf.Infinity;

        while (continueCameraMove)
        {
            Vector3 targetPos = forestGodFaceTransform.position;
            targetPos.y = playerCharacter.LookTransform.position.y;
            Vector3 targetVel = (targetPos - endSequence1View.transform.position).normalized * maxCameraVelocity;

            camVelocity = Vector3.SmoothDamp(camVelocity, targetVel, ref dampVelocity, cameraSmoothing);
            endSequence1View.transform.position += camVelocity * Time.deltaTime;

            Vector3 vecToTarget = forestGodFaceTransform.position - endSequence1View.transform.position;
            endSequence1View.transform.rotation = Quaternion.Slerp(initRotation, Quaternion.LookRotation(vecToTarget), lookAtAnim.Evaluate(timer));

            if(!fadeStarted)
            {
                if(vecToTarget.sqrMagnitude < part1EndRadius * part1EndRadius)
                {
                    // SYNC FADE WITH BLINKING
                    //MainCamera.Effects.SetFadeColor(Color.black);
                    //MainCamera.Effects.CrossFade(fadeDuration, true);
                    //fadeStarted = true;
                    //fadeStartTime = timer;
                    continueCameraMove = false;
                }
            }
            //else
            //{
            //    if (timer > fadeStartTime + fadeDuration)
            //    {
            //        continueCameraMove = false;
            //    }
            //}

            yield return null;
            timer += Time.deltaTime;
        }

        // End sequence part 2
        endSequence2View.RequestView();
        // unload forest, load in ghost dining hall

        yield return new WaitForSeconds(part2Duration);
        // End sequence part 3
        forestGeometry.SetActive(false);
        hallGeometry.SetActive(true);

        endSequence3View.RequestView();
        godLookAtTarget.Target = endSequence3View.transform;

        endSequence3Animator.SetTrigger("EndSequence3");

        yield return new WaitUntil(() => _endAnimReadyToEnd);

        // End sequence part 4
        MainCamera.Effects.SetFadeColor(Color.black);
        MainCamera.Effects.CrossFade(finalFadeOutDuration, true);
        yield return new WaitForSeconds(finalFadeOutDuration + blackOutLingerTime);
        // Fade out audio?
        PauseManager.Instance.PausingAllowed = true;
        GlobalData.HasCompletedGame = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
