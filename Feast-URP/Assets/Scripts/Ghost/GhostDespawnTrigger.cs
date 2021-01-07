using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostDespawnTrigger : LimitedTrigger
{
    [SerializeField] private GhostAI[] ghosts;
    [SerializeField] private float maxDespawnDelay = 0.3f;

    protected override void OnTrigger() => GhostManager.Instance.DespawnCollection(ghosts, maxDespawnDelay);

#if UNITY_EDITOR
    [ContextMenu("Get ghosts from children")]
    private void Editor_GetGhostsFromChildren() => ghosts = GetComponentsInChildren<GhostAI>();
#endif
}
