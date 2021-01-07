using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpawnTrigger : LimitedTrigger
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float maxSpawnDelay = 0.3f;

    protected override void OnTrigger() => GhostManager.Instance.SpawnCollection(spawnPoints, maxSpawnDelay);

#if UNITY_EDITOR
    [ContextMenu("Get spawnpoints from children")]
    private void Editor_GetGhostsFromChildren() => spawnPoints = GetComponentsInChildren<Transform>();
#endif
}
