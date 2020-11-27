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
        if (MainCamera.InstanceParent == transform)
            MainCamera.Unparent();
    }
}
