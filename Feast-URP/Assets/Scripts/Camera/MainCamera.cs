using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
#pragma warning disable 0649
    public static Transform RootTransform => IsValid() ? _instance._cameraRoot : null;
    public static Camera Camera => IsValid() ? _instance.camera : null;
    public static UniversalAdditionalCameraData CameraData => IsValid() ? _instance.cameraData : null;
    public static CameraFX Effects => IsValid() ? _instance.cameraEffects : null;
    public static CameraView CurrentView => IsValid() ? _instance._currentView : null;

    public static bool IsBlending => IsValid() && _instance._isBlending;
    public static bool IsValid() => _instance;

    private static MainCamera _instance;

    [SerializeField] private new Camera camera;
    [SerializeField] private UniversalAdditionalCameraData cameraData;
    [SerializeField] private VolumeProfile basePostProcessing;
    [SerializeField] private CameraFX cameraEffects;
    [SerializeField] private BlendOverrideCollection blendOverrides;

    private Transform _cameraRoot;
    private CameraView _currentView;
    private bool _isBlending;

    public static void RequestView(CameraView view)
    {
        if (!IsValid()) return;

        // Handle null view case
        if(!view)
        {
            RootTransform.SetParent(null);
            _instance._currentView = null;
            return;
        }

        // Figure out if blend needs to be overriden
        ViewBlend blend;
        if(!_instance.blendOverrides.TryGetBlendOverride(_instance._currentView, view, out blend))
        {
            blend = view.BlendSettings;
        }

        // Apply blend, if there is one
        if(blend)
        {
            // Running coroutine on incoming view in case view gets unloaded during blend
            _instance._isBlending = true;
            view.StartCoroutine(blend.CreateBlend(view, () => _instance._isBlending = false));
        }
        else
        {
            Util.MoveTransformToTarget(RootTransform, view.transform, true);
            _instance.camera.fieldOfView = view.FieldOfView;
        }

        _instance._currentView = view;
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
            blendOverrides.Initialize();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += PlaymodeCleanup;
#endif
        }
    }

    private void Update()
    {
        Effects.Update(Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;

            // Clean up any possible camera effects
            cameraEffects.ResetFadeColorToDefault();
            cameraEffects.CrossFade(0f, false);
            cameraEffects.SetColorInvert(false);
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
        Effects.SetColorInvert(false);

        // Clean up noise
        Effects.ResetCameraNoiseModifiers();
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
