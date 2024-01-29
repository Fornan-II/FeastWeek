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
    [SerializeField] private LayerMask cullingMask = Physics.AllLayers;
    [SerializeField] private UnityEvent OnBlobDetected = null;
    [SerializeField] private Vector2Int renderTextureSize = new Vector2Int(128, 128);
    [SerializeField] private int minDetectedPixels = 128;

    private RenderTexture _renderTexture;
    private Texture2D _readTexture;
    private bool _detectionActive = false;
    private Vector2 _cachedScreenSize;

    private void OnDestroy()
    {
        _renderTexture.Release();
    }

    public void StartDetection()
    {
        if(!_renderTexture)
        {
            _cachedScreenSize = new Vector2(Screen.width, Screen.height);
            _renderTexture = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 8, RenderTextureFormat.R8);
            _renderTexture.Create();

            _readTexture = new Texture2D(renderTextureSize.x, renderTextureSize.y, TextureFormat.R8, false);
        }

        Util.MoveTransformToTarget(renderCam.transform, MainCamera.Camera.transform, true);
        renderCam.CopyFrom(MainCamera.Camera);
        renderCam.cullingMask = cullingMask;
        renderCam.targetTexture = _renderTexture;
        renderCam.backgroundColor = Color.black;
        renderCam.clearFlags = CameraClearFlags.Color;
        // Set renderer if needed

        Debug.Log("Blob detection starting...");

        _detectionActive = true;
    }

    private void BlobDetected()
    {
        Debug.Log("Blob detected!");
        OnBlobDetected.Invoke();
        _detectionActive = false;

        if (deactiveGameObjectOnDetection)
            gameObject.SetActive(false);
    }
    
    private void LateUpdate()
    {
        if (!_detectionActive) return;
        if (CheckRenderTexture)
        {
            // https://answers.unity.com/questions/27968/getpixels-of-rendertexture.html
            // https://forum.unity.com/threads/rendertexture-readpixels-getpixel-shortcut.1103338/
            RenderTexture.active = _renderTexture;
            Rect textureRect = new Rect(0, 0, _readTexture.width, _readTexture.height);
            _readTexture.ReadPixels(textureRect, 0, 0);

            // GetPixelData should be the most efficient way to read pixel data
            // Doesn't copy any memory, gets entire array
            // Using byte because of R8 TextureFormat of RenderTexture
            Unity.Collections.NativeArray<byte> data = _readTexture.GetPixelData<byte>(0);
            int detectedPixelCount = 0;
            for(int i = 0; i < data.Length; ++i)
            {
                if (data[i] > 127)
                {
                    ++detectedPixelCount;
                    if (detectedPixelCount >= minDetectedPixels)
                    {
                        BlobDetected();
                        return;
                    }
                }
            }
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
