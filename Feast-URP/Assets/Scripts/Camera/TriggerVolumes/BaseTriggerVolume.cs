using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTriggerVolume : MonoBehaviour
{
    public bool IsOverlapping { get; private set; }
    private bool _wasOverlapping = false;

    private void OnDisable()
    {
        OnOverlapExit();
    }

    protected virtual void Update()
    {
        IsOverlapping = IsMainCameraWithin();

        if(IsOverlapping)
        {
            if (!_wasOverlapping)
                OnOverlapStart();
            OnOverlap();
        }
        else if(_wasOverlapping)
            OnOverlapExit();

        _wasOverlapping = IsOverlapping;
    }

    protected abstract bool IsMainCameraWithin();

    protected virtual void OnOverlapStart() { }
    protected virtual void OnOverlap() { }
    protected virtual void OnOverlapExit() { }
}
