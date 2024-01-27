using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GhostManager : MonoBehaviour
{
#pragma warning disable 0649
    public static GhostManager Instance { get; private set; }

    public bool GhostsAggroToPlayer;

    [SerializeField] private GhostAI ghostPrefab;
    [Header("Height randomization")]
    [SerializeField] private bool randomizeGhostSize;
    [SerializeField] private Vector2 randomSizeRange = Vector2.one;

    private Stack<GhostAI> _inactiveGhostPool = new Stack<GhostAI>();

    #region Unity Methods
    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    #endregion

    #region Pooling
    public GhostAI SpawnGhost(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GhostAI spawnedGhost;
        if(_inactiveGhostPool.Count > 0)
        {
            spawnedGhost = _inactiveGhostPool.Pop();
            spawnedGhost.transform.SetPositionAndRotation(position, rotation);
            spawnedGhost.transform.SetParent(parent);
            spawnedGhost.gameObject.SetActive(true);
        }
        else
        {
            spawnedGhost = Instantiate(ghostPrefab, position, rotation, parent);
        }

        // Randomize ghost size for variation
        if(randomizeGhostSize)
        {
            spawnedGhost.transform.localScale = Vector3.one * Util.RandomInRange(randomSizeRange);
        }

        return spawnedGhost;
    }

    public void DespawnGhost(GhostAI ghost) => StartCoroutine(GhostDespawnRoutine(ghost));

    private IEnumerator GhostDespawnRoutine(GhostAI ghost)
    {
        ghost.GhostVFX.BecomeInvisible();
        yield return new WaitUntil(() => !ghost.GhostVFX.IsVisible);
        ghost.gameObject.SetActive(false);
        ghost.transform.parent = transform;
        _inactiveGhostPool.Push(ghost);
    }
    #endregion

    #region Collection Spawning
    public void SpawnCollection(Transform[] spawnLocations, float maxDelay, UnityAction<GhostAI> spawnedGhostAction = null) => StartCoroutine(SpawnWithDelayCollection(spawnLocations, maxDelay, spawnedGhostAction));

    private IEnumerator SpawnWithDelayCollection(Transform[] spawnLocations, float maxDelay, UnityAction<GhostAI> spawnedGhostAction)
    {
        foreach (var t in spawnLocations)
        {
            GhostAI spawnedGhost = SpawnGhost(t.position, t.rotation);
            spawnedGhostAction?.Invoke(spawnedGhost);

            yield return new WaitForSeconds(Random.Range(0, maxDelay));
        }
    }

    public void DespawnCollection(GhostAI[] ghosts, float maxDelay, UnityAction<GhostAI> despawnedGhostAction = null) => StartCoroutine(DespawnWithDelayCollection(ghosts, maxDelay, despawnedGhostAction));

    private IEnumerator DespawnWithDelayCollection(GhostAI[] ghosts, float maxDelay, UnityAction<GhostAI> despawnedGhostAction)
    {
        foreach (var g in ghosts)
        {
            despawnedGhostAction?.Invoke(g);
            DespawnGhost(g);
            yield return new WaitForSeconds(Random.Range(0, maxDelay));
        }
    }
    #endregion
}
