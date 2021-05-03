using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpawnTrigger : LimitedTrigger
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GhostQuest quest;
    [SerializeField] private float maxSpawnDelay = 0.3f;

    private GhostAI _spawnedGhost;

    protected override void OnTrigger()
    {
        _spawnedGhost = GhostManager.Instance.SpawnGhost(spawnPoint.position, spawnPoint.rotation, spawnPoint);
        _spawnedGhost.SetQuest(quest);
    }

    public void DespawnSpawnedGhost()
    {
        if(_spawnedGhost)
        {
            GhostManager.Instance.DespawnGhost(_spawnedGhost);
        }
    }
}
