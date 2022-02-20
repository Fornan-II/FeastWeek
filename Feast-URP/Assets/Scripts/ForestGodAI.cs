using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGodAI : StateMachine
{
    public enum ForestGodState
    {
        NONE,
        SIT,
        SEEK_PLAYER
    }

    [Header("Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private LookAtTarget lookAt;
    [Header("State")]
    [SerializeField] private ForestGodState defaultState = ForestGodState.SIT;
    [SerializeField] private ForestGodState currentState = ForestGodState.NONE;

    public void TriggerPlayerAggro()
    {
        currentState = ForestGodState.SEEK_PLAYER;
        recalculateWhenReady = true;
    }

    #region StateMachine overrides
    protected override void RecalculateState()
    {
        // Figure out what state to run
        if(currentState == ForestGodState.NONE)
        {
            currentState = defaultState;
        }

        // Start running state
        switch (currentState)
        {
            case ForestGodState.SIT:
                _activeState = StartCoroutine(Sit());
                break;
            case ForestGodState.SEEK_PLAYER:
                _activeState = StartCoroutine(SeekPlayer());
                break;
        }
    }
    #endregion

    #region States
    private IEnumerator Sit()
    {
        anim.SetBool("IsSitting", true);
        while (currentState == ForestGodState.SIT)
        {
            yield return null;
        }
        anim.SetBool("IsSitting", false);

        _activeState = null;
    }

    private IEnumerator SeekPlayer()
    {
        while(true)
        {
            yield return null;
        }

        _activeState = null;
    }
    #endregion
}
