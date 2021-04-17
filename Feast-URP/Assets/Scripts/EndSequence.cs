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

    private IEnumerator Anim()
    {
        musicManager.enabled = false;
        musicManager.musicSource.clip = deathBlobMusic;
        musicManager.musicSource.Play();

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
            PauseManager.Instance.PausingAllowed = false;
            MainCamera.Effects.SetFadeColor(Color.black);
            MainCamera.Effects.CrossFade(0f, true);
            playerController.ReleaseControl();
            director.Stop();
            deathBlob.SetActive(false);
            castleDoor.CloseDoor();
            musicManager.FadeOut(7f);

            yield return new WaitForSeconds(2f);
            deathAnimation.SetTrigger("PlayAnimation");
            deathView.RequestView();
            MainCamera.Effects.SetColorInvert(true);
            MainCamera.Effects.CrossFade(5f, false);

            yield return new WaitForSeconds(12f);
            MainCamera.Effects.CrossFade(5f, true);

            yield return new WaitForSeconds(6f);
            MainCamera.Effects.SetColorInvert(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
