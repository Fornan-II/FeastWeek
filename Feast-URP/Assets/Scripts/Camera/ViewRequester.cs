using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRequester : MonoBehaviour
{
    public bool SetFOVEveryFrame = false;

    public float FieldOfView = 100;
    [SerializeField] private bool setFOV = false;

    public bool HasView() => MainCamera.RootTransform.parent == transform;

    [ContextMenu("Request view")]
    public void RequestView()
    {
        MainCamera.RequestView(transform);
        if(setFOV)
            MainCamera.SetFOV(FieldOfView);
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
            MainCamera.SetFOV(FieldOfView);
        }
    }
}
