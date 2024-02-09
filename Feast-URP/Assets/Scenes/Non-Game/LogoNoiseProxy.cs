using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LogoNoiseProxy : MonoBehaviour
{
    public AnimationCurve noiseAnim;
    public AnimationCurve volumeAnim;
    public Volume overrideVolume;
    
    // Start is called before the first frame update
    void Start()
    {
        RunNoiseAnim();
    }

    [ContextMenu("Run Noise Anim")]
    private void RunNoiseAnim()
    {
        if (!UnityEditor.EditorApplication.isPlaying) return;

        StartCoroutine(NoiseAnim());
    }

    private IEnumerator NoiseAnim()
    {
        float length = Mathf.Max(Util.AnimationCurveLengthTime(noiseAnim), Util.AnimationCurveLengthTime(volumeAnim));

        for(float t = 0f; t < length; t += Time.deltaTime)
        {
            MainCamera.Effects.ApplyCameraNoise(GetInstanceID(), noiseAnim.Evaluate(t));
            overrideVolume.weight = volumeAnim.Evaluate(t);
            yield return null;
        }

        MainCamera.Effects.ApplyCameraNoise(GetInstanceID(), noiseAnim.keys[noiseAnim.length - 1].value);
        overrideVolume.weight = volumeAnim.keys[volumeAnim.length - 1].value;
    }
}
