using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    public ViewBlend BlendSettings => blendSettings;

    [Header("Field Of View")]
    public bool SetFOVEveryFrame = false;
    public float FieldOfView = 100;

    [Header("View Blending")]
    [SerializeField] private ViewBlend blendSettings;

    public bool HasView() => MainCamera.CurrentView == this;

    [ContextMenu("Request view")]
    public void RequestView()
    {
        MainCamera.RequestView(this);
    }

    [ContextMenu("Release view")]
    public void ReleaseView()
    {
        if (HasView())
            MainCamera.RequestView(null);
    }

    private void Update()
    {
        if(SetFOVEveryFrame && HasView())
        {
            MainCamera.Camera.fieldOfView = FieldOfView;
        }
    }
}
