using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
    public static MainCamera Instance { get; private set; }
    public static Transform InstanceParent => Instance ? Instance.transform.parent : null;

    public Camera Camera => camera;
    public UniversalAdditionalCameraData CameraData => cameraData;

    [SerializeField] private new Camera camera;
    [SerializeField] private UniversalAdditionalCameraData cameraData;
    [SerializeField] private VolumeProfile basePostProcessing;

    public static void RequestView(Transform parent, Vector3 localPosition, Quaternion localRotation)
    {
        if (!Instance) return;
        Instance.transform.SetParent(parent);
        Instance.transform.localPosition = localPosition;
        Instance.transform.localRotation = localRotation;
    }

    public static void RequestView(Transform parent) => RequestView(parent, Vector3.zero, Quaternion.identity);

    public static void RequestView(Vector3 position, Quaternion rotation)
    {
        if (!Instance) return;
        Instance.transform.SetPositionAndRotation(position, rotation);
    }

    public static void Unparent()
    {
        if (!Instance) return;
        Instance.transform.parent = null;
    }

    private void Awake()
    {
        if(Instance)
        {
            Debug.LogWarning("Existing MainCamera Instance present! Destroying Original.");
            Destroy(Instance.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!camera) camera = GetComponent<Camera>();
        if (!cameraData) cameraData = GetComponent<UniversalAdditionalCameraData>();
        if (!basePostProcessing && TryGetComponent(out Volume camVolume))
        {
            basePostProcessing = camVolume.profile;
        }
    }
#endif
}
