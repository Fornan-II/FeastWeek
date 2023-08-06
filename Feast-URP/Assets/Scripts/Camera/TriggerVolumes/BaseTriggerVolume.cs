using UnityEngine;
using UnityEngine.Events;

public abstract class BaseTriggerVolume : MonoBehaviour
{
    public bool IsOverlapping { get; private set; }

    /// <summary>
    /// A 0 to 1 value indicating camera's position within radii. Set during Unity Update() call before overlap calls.
    /// Should be assigned to in overrides of IsMainCameraWithin
    /// </summary>
    public float BlendValue { get; protected set; } = 0f;

    protected event UnityAction _OnOverlapStart;
    protected event UnityAction _OnOverlap;
    protected event UnityAction _OnOverlapExit;

    private bool _wasOverlapping = false;

    public void AddListener(ITriggerListener listener)
    {
        _OnOverlapStart += listener.OnOverlapStart;
        _OnOverlap += listener.OnOverlap;
        _OnOverlapExit += listener.OnOverlapExit;
    }

    public void RemoveListener(ITriggerListener listener)
    {
        if (IsOverlapping)
            listener.OnOverlapExit();

        _OnOverlapStart -= listener.OnOverlapStart;
        _OnOverlap -= listener.OnOverlap;
        _OnOverlapExit -= listener.OnOverlapExit;
    }

    private void OnDisable()
    {
        if(IsOverlapping)
        {
            _OnOverlapExit?.Invoke();
            IsOverlapping = false;
        }
    }

    protected virtual void Update()
    {
        IsOverlapping = IsMainCameraWithin();

        if(IsOverlapping)
        {
            if (!_wasOverlapping)
                _OnOverlapStart?.Invoke();
            _OnOverlap?.Invoke();
        }
        else if(_wasOverlapping)
            _OnOverlapExit?.Invoke();

        _wasOverlapping = IsOverlapping;
    }

    protected abstract bool IsMainCameraWithin();
}
