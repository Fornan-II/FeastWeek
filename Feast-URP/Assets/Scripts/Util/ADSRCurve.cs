using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ADSRCurve : ISerializationCallbackReceiver
{
    [SerializeField] private AnimationCurve curve;

    private bool _isTriggered;
    private Coroutine _activeTimer;
    private float _timerValue = 0.0f;

    ADSRCurve()
    {
        ResetCurve();
    }

    ~ADSRCurve()
    {
        if (_activeTimer != null)
        {
            GameManager.Instance.StopCoroutine(_activeTimer);
        }
    }

    public void SendNoteOn()
    {
        _isTriggered = true;

        if(_activeTimer != null)
        {
            GameManager.Instance.StopCoroutine(_activeTimer);
        }
        _activeTimer = GameManager.Instance.StartCoroutine(RunTimer());
    }

    public void SendNoteOff() => _isTriggered = false;

    public float Evaluate() => curve.Evaluate(_timerValue);

    private IEnumerator RunTimer()
    {
        // Attack: curve.keys[1].time
        // Decay: curve.keys[2].time - curve.keys[1].time
        // Sustain: curve.keys[2].value
        // Release: curve.keys[3].time - curve.keys[2].time

        _timerValue = 0.0f;
        
        // Attack and Decay
        while(_isTriggered && _timerValue < curve.keys[2].time)
        {
            _timerValue += Time.deltaTime;
            yield return null;
        }

        _timerValue = curve.keys[2].time;

        // Wait for NoteOff
        while (_isTriggered)
        {
            yield return null;
        }

        // Release
        while(_timerValue < curve.keys[3].time)
        {
            _timerValue += Time.deltaTime;
            yield return null;
        }

        _timerValue = curve.keys[3].time;
        _activeTimer = null;
    }

    [ContextMenu("Reset curve")]
    public void ResetCurve()
    {
        curve = new AnimationCurve(new Keyframe[]
        {
            new Keyframe(0.0f, 0.0f),
            new Keyframe(0.1f, 1.0f),
            new Keyframe(0.1f, 0.5f),
            new Keyframe(0.25f, 0.0f)
        });
    }
    
    public void OnBeforeSerialize()
    {
        // Ensure length
        if (curve.keys.Length == 0)
        {
            ResetCurve();
        }
        else if(curve.keys.Length != 4)
        {
            while (curve.keys.Length > 4)
            {
                curve.RemoveKey(4);
            }

            while (curve.keys.Length < 4)
            {
                curve.AddKey(Util.AnimationCurveLengthTime(curve) + 0.25f, 0f);
            }
        }

        // Enforce key time/value rules
        curve.keys[0].time = 0.0f;
        curve.keys[0].value = 0.0f;

        curve.keys[1].value = Mathf.Clamp01(curve.keys[1].value);

        curve.keys[2].value = Mathf.Clamp01(curve.keys[1].value);

        curve.keys[3].value = 0.0f;
    }

    public void OnAfterDeserialize() { }
}
