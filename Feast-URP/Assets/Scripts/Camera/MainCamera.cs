using System.Collections;
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

    private Coroutine _activeFadeRoutine;

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

    public static void FadeScreen(float fadeDuration, bool fadeToWhite = false)
    {
        if(fadeDuration <= 0f)
            Shader.SetGlobalFloat("_ScreenFade", fadeToWhite ? 1f : 0f);
        else
        {
            if (Instance._activeFadeRoutine != null) Instance.StopCoroutine(Instance._activeFadeRoutine);
            Instance._activeFadeRoutine = Instance.StartCoroutine(Instance.FadeScreen(fadeToWhite ? 0f : 1f, fadeToWhite ? 1f : 0f, fadeDuration));
        }
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

    private IEnumerator FadeScreen(float from, float to, float duration)
    {
        for(float timer = 0.0f; timer < duration; timer += Time.deltaTime)
        {
            float tValue = Mathf.Lerp(from, to, timer / duration);
            Shader.SetGlobalFloat("_ScreenFade", tValue * tValue);
            yield return null;
        }
        Shader.SetGlobalFloat("_ScreenFade", to);
        _activeFadeRoutine = null;
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
