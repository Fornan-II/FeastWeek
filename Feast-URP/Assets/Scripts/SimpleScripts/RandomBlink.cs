using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBlink : MonoBehaviour
{
#pragma warning disable 0649
    public bool IsBlinking => _isBlinking;

    [SerializeField] private Transform eyeTransform;
    [SerializeField] private Vector2 BlinkIntervalRange = new Vector2(1f, 30f);
    [SerializeField] private AnimationCurve blinkAnim;

    private Vector3 _eyeDefaultScale;
    private float _blinkIntervalTimer;
    private bool _isBlinking = false;
    private bool _initialized = false;

    private void Awake() => Initialize();
    private void OnEnable() => _blinkIntervalTimer = Util.RandomInRange(BlinkIntervalRange);

    private void OnDisable()
    {
        _isBlinking = false;
        eyeTransform.localScale = _eyeDefaultScale;
    }

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

    public void SetOpenness(float openness01)
    {
        Initialize();
        openness01 = Mathf.Clamp01(openness01);

        eyeTransform.localScale = new Vector3(_eyeDefaultScale.x, openness01 * _eyeDefaultScale.y, _eyeDefaultScale.z);

        if (openness01 <= 0.0f)
            eyeTransform.gameObject.SetActive(false);
        else if (!eyeTransform.gameObject.activeSelf)
            eyeTransform.gameObject.SetActive(true);
    }

    private void Initialize()
    {
        if (!_initialized)
        {
            _eyeDefaultScale = eyeTransform.localScale;
            _initialized = true;
        }
    }

    private IEnumerator Blink()
    {
        _isBlinking = true;
        float animLength = Util.AnimationCurveLengthTime(blinkAnim);
        for(float timer = 0.0f; timer < animLength; timer += Time.deltaTime)
        {
            float yScale = _eyeDefaultScale.y * blinkAnim.Evaluate(timer);
            SetOpenness(yScale);
            yield return null;
        }
        eyeTransform.localScale = _eyeDefaultScale;
        _isBlinking = false;
    }
}
