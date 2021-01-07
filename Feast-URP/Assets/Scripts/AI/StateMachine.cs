using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public bool recalculateIfNoState = true;
    protected Coroutine _activeState;
    protected Coroutine _activeInterruptWatcher;

    #region Unity Methods
    protected virtual void Update()
    {
        if(recalculateIfNoState && _activeState == null)
        {
            RecalculateState();
        }
    }

    protected virtual void OnDisable()
    {
        _activeState = null;
        _activeInterruptWatcher = null;
    }
    #endregion

    public virtual void StopState()
    {
        if (_activeState != null)
        {
            StopCoroutine(_activeState);
            _activeState = null;
        }
    }

    protected void InitInterruptWatcher()
    {
        if (_activeInterruptWatcher != null)
            StopCoroutine(_activeInterruptWatcher);
        _activeInterruptWatcher = StartCoroutine(InterruptWatcher());
    }

    protected virtual void RecalculateState()
    {

    }

    protected virtual IEnumerator InterruptWatcher()
    {
        Debug.LogError("By default this coroutine does nothing. Override with desired functionality.");
        yield return null;
        _activeInterruptWatcher = null;
    }
}
