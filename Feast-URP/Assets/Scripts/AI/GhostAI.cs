using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostAI : StateMachine
{
    public enum GhostState
    {
        NONE,
        IDLE,
        WANDER,
        WALK_TO,
        GRAB
    }

    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator anim;
    [SerializeField] private AnimatedCharacter character;
    [SerializeField] private LookAtTarget lookAt;
    [Header("State")]
    [SerializeField] private GhostState _defaultState = GhostState.IDLE;
    [SerializeField] private GhostState _currentState;

    [Header("State-Related Variables")]
    public bool WanderWhenIdling = false;
    [SerializeField] private Vector2 wanderIdleTime = new Vector2(1.0f, 7.0f);
    [SerializeField] private float wanderRadius = 7f;
    public Vector3 walkTarget;
    public bool IsAggroToPlayer;
    [SerializeField] private float grabRange = 0.3f;

    private bool _grabAnimTriggered = false;
    private bool _grabAnimComplete = false;

    #region Unity Methods
    private void Awake() => agent.updateRotation = false;
    private void Start() => lookAt.Target = MainCamera.Instance.transform;

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
        if(agent.isStopped || agent.remainingDistance <= agent.stoppingDistance)
        {
            character.Move(Vector3.zero, false, false);
        }
        else
        {
            character.Move(agent.desiredVelocity, false, false);
        }
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
        // Figure out what state to run
        if (_currentState == GhostState.NONE)
        {
            _currentState = _defaultState;
        }
        else if(IsAggroToPlayer)
        {
            walkTarget = PlayerRef.Transform.position;
            // probably want to change this to be set in some sensory way. Only when on screen or nearby?
            // Also would want to have this update if the player moves from their position
            // Can control allt this by changing IsAggroToPlayer
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
            // If the ghost lost aggro to player immediately after trying to attack that would be a bit lame
            IsAggroToPlayer = true;
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
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!anim) anim = GetComponent<Animator>();
        if (!character) character = GetComponent<AnimatedCharacter>();
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