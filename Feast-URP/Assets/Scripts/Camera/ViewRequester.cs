using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRequester : MonoBehaviour
{
    [SerializeField] private bool setFOV = false;
    [SerializeField] private float fieldOfView = 100;

    public void RequestView()
    {
        MainCamera.RequestView(transform);
        if(setFOV)
            MainCamera.SetFOV(fieldOfView);
    }

    public void ReleaseView()
    {
        if (MainCamera.RootTransform.parent == transform)
            MainCamera.RootTransform.SetParent(null);
    }
}
