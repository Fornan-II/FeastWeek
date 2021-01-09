using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRequester : MonoBehaviour
{
    public void RequestView()
    {
        MainCamera.RequestView(transform);
    }

    public void ReleaseView()
    {
        if (MainCamera.RootTransform.parent == transform)
            MainCamera.RootTransform.SetParent(null);
    }
}
