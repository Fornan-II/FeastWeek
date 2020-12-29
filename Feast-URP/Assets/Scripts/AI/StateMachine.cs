using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public bool recalculateIfNoState = true;
    protected Coroutine _activeState;

    protected virtual void Update()
    {
        if(recalculateIfNoState && _activeState == null)
        {
            RecalculateState();
        }
    }

    protected virtual void OnDisable() => _activeState = null;

    protected virtual void RecalculateState()
    {

    }

    public virtual void StopState()
    {
        if(_activeState != null)
        {
            StopCoroutine(_activeState);
            _activeState = null;
        }
    }
}
