using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostAI : StateMachine
{
#pragma warning disable 0649
    public enum GhostState
    {
        NONE,
        IDLE,
        WANDER,
        WALK_TO,
        GRAB
    }

    public GhostVFX GhostVFX => ghostVFX;

    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator anim;
    [SerializeField] private AnimatedCharacter character;
    [SerializeField] private LookAtTarget lookAt;
    [SerializeField] private GhostVFX ghostVFX;
    [Header("State")]
    [SerializeField] private GhostState _defaultState = GhostState.IDLE;
    [SerializeField] private GhostState _currentState;

    [Header("State-Related Variables")]
    public bool WanderWhenIdling = false;
    [SerializeField] private Vector2 wanderIdleTime = new Vector2(1.0f, 7.0f);
    [SerializeField] private float wanderRadius = 7f;
    public Vector3 walkTarget;
    [SerializeField] private float grabRange = 0.3f;

    private bool _grabAnimTriggered = false;
    private bool _grabAnimComplete = false;

    #region Unity Methods
    private void Awake() => agent.updateRotation = false;
    private void Start() => lookAt.Target = MainCamera.RootTransform;

    protected override void OnDisable()
    {
        base.OnDisable();
        agent.isStopped = true;
        _grabAnimTriggered = false;
        _grabAnimComplete = false;
    }

    protected override void Update()
    {
        base.Update();

        // Pass movement info for animation
        if(agent.isStopped || agent.remainingDistance <= agent.stoppingDistance)
            character.Move(Vector3.zero, false, false);
        else
            character.Move(agent.desiredVelocity, false, false);
    }
    #endregion

    #region StateMachine Overrides
    public override void StopState()
    {
        base.StopState();
        agent.isStopped = true;
    }

    protected override void RecalculateState()
    {
        InitInterruptWatcher();

        // Figure out what state to run
        if (_currentState == GhostState.NONE)
        {
            _currentState = _defaultState;
        }
        else if(GhostManager.Instance.GhostsAggroToPlayer)
        {
            walkTarget = PlayerRef.Transform.position;
            // probably want to change this to be set in some sensory way. Only when on screen or nearby?
            // Also would want to have this update if the player moves from their position
            // Can control all this by changing IsAggroToPlayer
            if((walkTarget - agent.transform.position).sqrMagnitude < grabRange * grabRange)
            {
                _currentState = GhostState.GRAB;
            }
            else
            {
                _currentState = GhostState.WALK_TO;
            }
        }
        else
        {
            if(WanderWhenIdling && _currentState == GhostState.IDLE)
            {
                _currentState = GhostState.WANDER;
            }
            else
            {
                _currentState = GhostState.IDLE;
            }
        }

        // Start running state
        switch(_currentState)
        {
            case GhostState.IDLE:
                _activeState = StartCoroutine(Idle());
                break;
            case GhostState.WANDER:
                _activeState = StartCoroutine(Wander());
                break;
            case GhostState.WALK_TO:
                _activeState = StartCoroutine(WalkTo());
                break;
            case GhostState.GRAB:
                _activeState = StartCoroutine(Grab());
                break;
        }
    }

    protected override IEnumerator InterruptWatcher()
    {
        // Set up vars to watch
        bool starterAggroState = GhostManager.Instance.GhostsAggroToPlayer;

        // Wait for a change in the vars
        while (starterAggroState == GhostManager.Instance.GhostsAggroToPlayer)
            yield return null;

        // Stop the current state to recalculate
        StopState();

        // Clear watcher coroutine
        _activeInterruptWatcher = null;
    }
    #endregion

    private void GrabAnimGrab() => _grabAnimTriggered = true;
    private void GrabAnimComplete() => _grabAnimComplete = true;

    #region States
    private IEnumerator Idle()
    {
        while (!WanderWhenIdling)
        {
            // Don't automatically leave this state
            yield return null;
        }

        for(float timer = Util.RandomInRange(wanderIdleTime); timer > 0f; timer -= Time.deltaTime)
        {
            yield return null;
        }

        _activeState = null;
    }

    private IEnumerator Wander()
    {
        walkTarget = Util.XZVector3(Random.insideUnitCircle * wanderRadius) + agent.transform.position;
        yield return WalkTo();
        //_activeState = null set by WalkTo()
    }

    private IEnumerator WalkTo()
    {
        agent.SetDestination(walkTarget);
        agent.isStopped = false;

        while (agent.pathPending)
            yield return null;

        while ( agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        agent.isStopped = true;
        _activeState = null;
    }

    private IEnumerator Grab()
    {
        anim.SetTrigger("Grab");
        while(!_grabAnimTriggered)
            yield return null;
        _grabAnimTriggered = false;

        if ((PlayerRef.Transform.position - agent.transform.position).sqrMagnitude < grabRange * grabRange)
        {
            // Successful grab
            // Player death anim or W/E happens here
            Debug.Log("Player grabbed");
        }
        else
        {
            // IF GHOSTS NEED TO SEE PLAYER:
            // Manually make sure ghosts are aware of player, because immediately losing track of player after attacking is weird/lame
        }

        while (!_grabAnimComplete)
            yield return null;
        _grabAnimComplete = false;

        _activeState = null;
    }
    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        agent = agent ?? GetComponent<NavMeshAgent>();
        anim = anim ?? GetComponent<Animator>();
        character = character ?? GetComponent<AnimatedCharacter>();
        ghostVFX = ghostVFX ?? GetComponent<GhostVFX>();
    }

    private void OnDrawGizmosSelected()
    {
        if(agent && agent.hasPath)
        {
            Gizmos.color = Color.blue;
            Vector3 prevPos = agent.transform.position;
            foreach(var pos in agent.path.corners)
            {
                Gizmos.DrawLine(prevPos, pos);
                prevPos = pos;
            }

            Vector3 finalPoint = agent.path.corners[agent.path.corners.Length - 1];
            Gizmos.DrawWireSphere(finalPoint, 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(finalPoint, walkTarget);
            Gizmos.DrawWireSphere(walkTarget, 0.1f);
        }
    }
#endif
}