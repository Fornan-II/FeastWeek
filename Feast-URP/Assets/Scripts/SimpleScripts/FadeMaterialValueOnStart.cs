using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMaterialValueOnStart : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private AnimationCurve materialValueFade;
    [SerializeField] private string materialProperty;
    
    private void OnEnable() => StartCoroutine(FadeRoutine());

    private IEnumerator FadeRoutine()
    {
        int propertyID = Shader.PropertyToID(materialProperty);
        meshRenderer.material.SetFloat(propertyID, materialValueFade.Evaluate(0f));

        float animLength = Util.AnimationCurveLengthTime(materialValueFade);
        for (float timer = 0.0f; timer < animLength; timer += Time.deltaTime)
        {
            yield return null;
            // Not use of .material instead of .sharedMaterial
            meshRenderer.material.SetFloat(propertyID, materialValueFade.Evaluate(timer));
        }

        meshRenderer.material.SetFloat(propertyID, materialValueFade.Evaluate(animLength));
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
    }
#endif
}
