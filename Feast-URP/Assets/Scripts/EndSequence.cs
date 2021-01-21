using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSequence : MonoBehaviour
{
    [SerializeField] private UnityEngine.Playables.PlayableDirector director;
    [SerializeField] private MeshRenderer ghostBones;
    [SerializeField] private GameObject alembicBlob;
    [SerializeField] private AnimationCurve ghostBonesAnimation = AnimationCurve.Linear(0f, -1f, 3f, 4f);
    [SerializeField] private float alembicBlobStartTime = 2.75f;
    [SerializeField] private Controller playerController;

    private bool _hasBeenActivated = false;

    private void Start()
    {
        alembicBlob.SetActive(false);
        ghostBones.material.SetFloat("_Extrusion", -1f);
    }

    public void Activate()
    {
        if (_hasBeenActivated) return;

        _hasBeenActivated = true;
        StartCoroutine(Anim());
    }

    private IEnumerator Anim()
    {
        bool directorStarted = false;
        float animLength = Util.AnimationCurveLengthTime(ghostBonesAnimation);
        for (float timer = 0.0f; timer < animLength; timer += Time.deltaTime)
        {
            ghostBones.material.SetFloat("_Extrusion", ghostBonesAnimation.Evaluate(timer));
            if(timer >= alembicBlobStartTime && !directorStarted)
            {
                alembicBlob.SetActive(true);
                director.Play();
            }
            yield return null;
        }
        
        
        //blitMat.SetColor("_FadeColor", Color.black);
        //MainCamera.Effects.CrossFade(fadeTime, true);

        //Vector3 startPos = MainCamera.RootTransform.position;
        //Quaternion startRot = MainCamera.RootTransform.rotation;
        //for(float timer = 0.0f; timer < animTime; timer += Time.deltaTime)
        //{
        //    float tValue = timer / animTime;
        //    tValue *= tValue;
        //    MainCamera.RootTransform.SetPositionAndRotation(
        //        Vector3.Lerp(startPos, camHolder.position, tValue),
        //        Quaternion.Slerp(startRot, camHolder.rotation, tValue)
        //        );
        //    yield return null;
        //}
        //MainCamera.RootTransform.SetPositionAndRotation(
        //        camHolder.position,
        //        camHolder.rotation
        //        );

        //yield return new WaitForSeconds(Mathf.Max(2f, fadeTime - animTime));

        //boneCamera.enabled = false;
    }
}
