using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSequence : MonoBehaviour
{
    [SerializeField] private Controller playerController;
    [SerializeField] private GameObject bones;
    [SerializeField] private Camera boneCamera;
    [SerializeField] private Material blitMat;
    [SerializeField] private Transform camHolder;
    [SerializeField] private float animTime = 4f;
    [SerializeField] private float fadeTime = 5f;

    private Color defaultBlitCol;

    private void Start()
    {
        defaultBlitCol = blitMat.GetColor("_FadeColor");
    }

    private void OnDestroy()
    {
        blitMat.SetColor("_FadeColor", defaultBlitCol);
    }

    public void Activate()
    {
        PauseManager.PausingAllowed = false;

        playerController.TakeControlOf(null);
        
        MainCamera.Instance.CameraData.cameraStack.Add(boneCamera);

        boneCamera.transform.parent = MainCamera.Instance.transform;
        boneCamera.transform.localPosition = Vector3.zero;
        boneCamera.transform.localRotation = Quaternion.identity;

        bones.layer = LayerMask.NameToLayer("Special");

        StartCoroutine(Anim());
    }

    private IEnumerator Anim()
    {
        blitMat.SetColor("_FadeColor", Color.black);
        MainCamera.FadeScreen(fadeTime, true);

        Vector3 startPos = MainCamera.Instance.transform.position;
        Quaternion startRot = MainCamera.Instance.transform.rotation;
        for(float timer = 0.0f; timer < animTime; timer += Time.deltaTime)
        {
            float tValue = timer / animTime;
            tValue *= tValue;
            MainCamera.Instance.transform.SetPositionAndRotation(
                Vector3.Lerp(startPos, camHolder.position, tValue),
                Quaternion.Slerp(startRot, camHolder.rotation, tValue)
                );
            yield return null;
        }
        MainCamera.Instance.transform.SetPositionAndRotation(
                camHolder.position,
                camHolder.rotation
                );

        yield return new WaitForSeconds(Mathf.Max(2f, fadeTime - animTime));

        boneCamera.enabled = false;

        yield return new WaitForSeconds(5f);
        Application.Quit();
        Debug.Log("quit");
    }
}
