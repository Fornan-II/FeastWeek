using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRequester : MonoBehaviour
{
    public bool SetFOVEveryFrame = false;

    [SerializeField] private bool setFOV = false;
    [SerializeField] private float fieldOfView = 100;

    public bool HasView() => MainCamera.RootTransform.parent == transform;

    public void RequestView()
    {
        MainCamera.RequestView(transform);
        if(setFOV)
            MainCamera.SetFOV(fieldOfView);
    }

    public void ReleaseView()
    {
        if(HasView())
            MainCamera.RootTransform.SetParent(null);
    }

    private void Update()
    {
        if(SetFOVEveryFrame && HasView())
        {
            MainCamera.SetFOV(fieldOfView);
        }
    }
}
