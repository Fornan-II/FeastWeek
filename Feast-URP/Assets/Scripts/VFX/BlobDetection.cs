using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class BlobDetection : MonoBehaviour
{
    public bool CheckRenderTexture = true;

    [SerializeField] private bool deactiveGameObjectOnDetection = true;
    [SerializeField] private Camera renderCam;
    [SerializeField] private UniversalAdditionalCameraData renderCamData;
    [SerializeField] private LayerMask cullingMask;
    [SerializeField] private UnityEvent OnBlobDetected;
    [SerializeField,Range(0,1)] private float renderTextureScalar = 1f;

    private RenderTexture _renderTexture;
    private bool _detectionActive = false;
    private Vector2 _cachedScreenSize;
    [SerializeField] private Vector2 rtWL;
    private bool _lastFrameWasInside = false;

    public void StartDetection()
    {
        if(!_renderTexture)
        {
            _cachedScreenSize = new Vector2(Screen.width, Screen.height);
            _renderTexture = new RenderTexture(Mathf.FloorToInt(Screen.width * renderTextureScalar), Mathf.FloorToInt(Screen.height * renderTextureScalar), 8, RenderTextureFormat.R8);
            _renderTexture.Create();
        }

        Util.MoveTransformToTarget(renderCam.transform, MainCamera.Camera.transform, true);
        renderCam.CopyFrom(MainCamera.Camera);
        renderCam.cullingMask = cullingMask;
        renderCam.targetTexture = _renderTexture;
        renderCam.backgroundColor = Color.black;
        renderCam.clearFlags = CameraClearFlags.Color;
        // Set renderer if needed

        _detectionActive = true;
    }

    private void BlobDetected()
    {
        OnBlobDetected.Invoke();
        _detectionActive = false;

        if (deactiveGameObjectOnDetection)
            gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!_detectionActive) return;
        if (_renderTexture) rtWL = new Vector2(_renderTexture.width, _renderTexture.height);
        if (CheckRenderTexture)
        {
            // https://answers.unity.com/questions/27968/getpixels-of-rendertexture.html
            RenderTexture.active = _renderTexture;
            Rect textureRect = new Rect(0, 0, _renderTexture.width, _renderTexture.height);
            Texture2D readTexture = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.R8, false);
            readTexture.ReadPixels(textureRect, 0, 0);

            for (int x = 0; x < _renderTexture.width; ++x)
            {
                for (int y = 0; y < _renderTexture.height; ++y)
                {
                    if (readTexture.GetPixel(x, y).r > 0.5f)
                    {
                        if (_lastFrameWasInside)
                            BlobDetected();
                        else
                            _lastFrameWasInside = true;
                        return;
                    }
                }
            }
        }

        _lastFrameWasInside = false;

        if(_renderTexture && (_cachedScreenSize.x != Screen.width || _cachedScreenSize.y != Screen.height))
        {
            _cachedScreenSize = new Vector2(Screen.width, Screen.height);
            _renderTexture.Release();
            _renderTexture.width = Mathf.FloorToInt(Screen.width * renderTextureScalar);
            _renderTexture.height = Mathf.FloorToInt(Screen.height * renderTextureScalar);
            _renderTexture.Create();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!renderCam) renderCam = GetComponent<Camera>();
        if (!renderCamData) renderCamData = GetComponent<UniversalAdditionalCameraData>();
    }
#endif
}
