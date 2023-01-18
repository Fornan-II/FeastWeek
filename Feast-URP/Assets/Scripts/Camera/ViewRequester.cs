using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRequester : MonoBehaviour
{
    [Header("Field Of View")]
    public bool SetFOVEveryFrame = false;
    public float FieldOfView = 100;
    [SerializeField] private bool setFOV = false;

    [Header("View Blending")]
    [SerializeField] private bool useViewBlending = false;
    [SerializeField] private AnimationCurve viewBlend = AnimationCurve.EaseInOut(0f, 0f, 0.25f, 1f);

    public bool HasView() => MainCamera.RootTransform.parent == transform;

    public ViewData MakeViewData()
    {
        ViewData data = new ViewData(transform);
        if (setFOV)
            data.FieldOfView = FieldOfView;
        return data;
    }

    [ContextMenu("Request view")]
    public void RequestView()
    {
        if(useViewBlending)
        {
            ViewBlend blend = new ViewBlend(MakeViewData(), viewBlend);
            StartCoroutine(blend.GetTransition());
        }
        else
        {
            MainCamera.RequestView(MakeViewData());
        }
    }

    [ContextMenu("Release view")]
    public void ReleaseView()
    {
        if(HasView())
            MainCamera.RootTransform.SetParent(null);
    }

    private void Update()
    {
        if(SetFOVEveryFrame && HasView())
        {
            MainCamera.Camera.fieldOfView = FieldOfView;
        }
    }
}
