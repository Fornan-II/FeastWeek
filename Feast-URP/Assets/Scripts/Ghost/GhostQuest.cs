using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GhostQuest
{
    [System.Serializable]
    public struct Goal
    {
        public GhostAI.GhostState GoalState;
        // Different properties display depending on state?
        [Tooltip("Used by Idle & Wander")] public float Duration;
        [Tooltip("Used by Walk to")] public Transform WalkTarget;
        [Tooltip("Used by Walk to")] public float walkTargetProximity;

        public UnityEvent OnComplete;
    }

    public Goal[] Goals;

    public int GoalCount => Goals.Length;
}
