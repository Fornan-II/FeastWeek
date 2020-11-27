using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBlink : MonoBehaviour
{
    [SerializeField] private Transform eyeTransform;
    [SerializeField] private Vector2 BlinkIntervalRange = new Vector2(1f, 30f);
    [SerializeField] private AnimationCurve blinkAnim;

    private float _blinkIntervalTimer;
    private bool _isBlinking = false;

    private void Start() => _blinkIntervalTimer = Util.RandomInRange(BlinkIntervalRange);

    void Update()
    {
        if(_blinkIntervalTimer <= 0.0f && !_isBlinking)
        {
            _blinkIntervalTimer = Util.RandomInRange(BlinkIntervalRange);
            StartCoroutine(Blink());
        }
        else
        {
            _blinkIntervalTimer -= Time.deltaTime;
        }
    }

    private IEnumerator Blink()
    {
        _isBlinking = true;
        float animLength = Util.AnimationCurveLengthTime(blinkAnim);
        Vector3 originalScale = eyeTransform.localScale;
        for(float timer = 0.0f; timer < animLength; timer += Time.deltaTime)
        {
            float yScale = originalScale.y * blinkAnim.Evaluate(timer);
            eyeTransform.localScale = new Vector3(originalScale.x, yScale, originalScale.z);
            if(yScale <= 0.0f)
                eyeTransform.gameObject.SetActive(false);
            else if(!eyeTransform.gameObject.activeSelf)
                eyeTransform.gameObject.SetActive(true);
            yield return null;
        }
        eyeTransform.localScale = originalScale;
        _isBlinking = false;
    }
}
