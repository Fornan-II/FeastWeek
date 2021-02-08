using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
    public static Transform RootTransform => _instance ? _instance._cameraRoot : null;
    public static Camera Camera => _instance ? _instance.camera : null;
    public static UniversalAdditionalCameraData CameraData => _instance ? _instance.cameraData : null;
    public static CameraFX Effects => _instance ? _instance.cameraEffects : null;

    private static MainCamera _instance;

    [SerializeField] private new Camera camera;
    [SerializeField] private UniversalAdditionalCameraData cameraData;
    [SerializeField] private VolumeProfile basePostProcessing;
    [SerializeField] private CameraFX cameraEffects;

    private Transform _cameraRoot;

    public static void RequestView(Transform parent, Vector3 localPosition, Quaternion localRotation)
    {
        if (!_instance) return;
        _instance._cameraRoot.SetParent(parent);
        _instance._cameraRoot.localPosition = localPosition;
        _instance._cameraRoot.localRotation = localRotation;
    }

    public static void RequestView(Transform parent) => RequestView(parent, Vector3.zero, Quaternion.identity);

    public static void RequestView(Vector3 position, Quaternion rotation)
    {
        if (!_instance) return;
        _instance._cameraRoot.SetPositionAndRotation(position, rotation);
    }

    #region Unity Methods
    private void Awake()
    {
        if(_instance)
        {
            Debug.LogWarning("Existing MainCamera Instance present! Destroying Original.");
            Destroy(_instance.gameObject);
        }
        else
        {
            _instance = this;
            _cameraRoot = new GameObject("MainCameraRoot").transform;
            Util.MoveTransformToTarget(_cameraRoot, transform);
            transform.SetParent(_cameraRoot);
            cameraEffects.Init(this);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += PlaymodeCleanup;
#endif
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= PlaymodeCleanup;
#endif
        }
    }
    #endregion

#if UNITY_EDITOR
    private void PlaymodeCleanup(UnityEditor.PlayModeStateChange playModeStateChange)
    {
        if (playModeStateChange != UnityEditor.PlayModeStateChange.ExitingPlayMode) return;

        // Clean up screen fade
        Effects.ResetFadeColorToDefault();
        Effects.CrossFade(0f, false);
    }

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
