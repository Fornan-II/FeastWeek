using System;
using UnityEngine;

// https://twitter.com/HackTrout/status/1362008498444320770?s=20&t=i2afNUCHo24IFw0zLDmbdw

[Serializable]
public class Chain
{
    [Serializable]
    public class Node
    {
        public Vector3 Position = Vector3.zero;
        public Vector3 Forward = Vector3.forward;
        public bool UsePhysics = false;

        public Vector3 Velocity { get; private set; } = Vector3.zero;

        public Node() { }

        public Node(Vector3 position, bool usePhysics = true)
        {
            Position = position;
            UsePhysics = usePhysics;
        }

        public Node(Node other)
        {
            Position = other.Position;
            UsePhysics = other.UsePhysics;
            Velocity = other.Velocity;
        }

        public void ProcessPhysics(float deltaTime)
        {
            if (!UsePhysics) return;
            Position += Velocity * deltaTime;
        }

        public void ApplyForce(Vector3 force)
        {
            if (!UsePhysics) return;
            Velocity += force;
        }
    }

    public int PointCount => _pointCount;
    public bool Initialized => _nodes != null;
    
    [Min(0)] public float stiffness = 5;
    [Min(0)] public float lengthScaler = 1.2f;
    [Min(0), Tooltip("Product of fluid density, cross sectional area, and drag coefficient.")] public float Drag = 0f;
    public bool UseGravityOverride = false;
    public Vector3 GravityOverride = Vector3.zero;

#if UNITY_EDITOR
    [Header("DEBUG")]
    [SerializeField] private int _debugSelectIndex = -1;
#endif

    protected int _pointCount;
    protected Node[] _nodes;
    protected float _targetLength;
    
    protected Action<int, Node> _modifyNodeAction;

    public Chain() { }

    public Chain(Chain other, int startIndex = 0, int lastIndex = -1)
    {
        // Copy over chain properties
        stiffness = other.stiffness;
        lengthScaler = other.lengthScaler;
        Drag = other.Drag;
        UseGravityOverride = other.UseGravityOverride;
        GravityOverride = other.GravityOverride;
        _modifyNodeAction = other._modifyNodeAction;

        // Copy over chain nodes
        if (lastIndex < 0)
        {
            lastIndex = other.PointCount - 1;
        }

        _pointCount = lastIndex - startIndex + 1;
        _nodes = new Node[_pointCount];

        for (int i = 0; i < _nodes.Length; ++i)
        {
            _nodes[i] = new Node(other._nodes[i + startIndex]);
        }
    }

    public virtual void Initialize(int pointCount, Action<Node[]> createNodeFunc)
    {
        _pointCount = pointCount;
        _nodes = new Node[_pointCount];

        createNodeFunc(_nodes);
        _targetLength = (_nodes[pointCount - 1].Position - _nodes[0].Position).magnitude / pointCount;
    }

    public virtual void InitializeLine(int pointCount, Vector3 startPosition, bool startIsFixed, Vector3 endPosition, bool endIsFixed)
    {
        Vector3 deltaPos = (endPosition - startPosition) / pointCount;

        Initialize(pointCount, (Node[] nodes) =>
        {
            for(int i = 0; i < nodes.Length; ++i)
            {
                bool usePhysics = true;
                if (i == 0) usePhysics = !startIsFixed;
                if (i == pointCount - 1) usePhysics = !endIsFixed;

                nodes[i] = new Node(
                   startPosition + deltaPos * i,
                   usePhysics
               );
            }
        });
    }

    public virtual void ProcessPhysics(float deltaTime)
    {
        // Update positions of nodes seperately from force calculations
        foreach (Node n in _nodes)
        {
            n.ProcessPhysics(deltaTime);
        }
        
        for (int i = 0; i < _nodes.Length; ++i)
        {
            // Apply chain forces to nodes
            // Checking the "chain" between the ith node and the node i + 1
            if (i + 1 < _nodes.Length)
            {
                Vector3 deltaPos = _nodes[i + 1].Position - _nodes[i].Position;
                float magnitude = (deltaPos.magnitude - _targetLength * lengthScaler) * stiffness;
                deltaPos.Normalize();

                _nodes[i].ApplyForce(deltaPos * magnitude);
                _nodes[i + 1].ApplyForce(deltaPos * -magnitude);

#if UNITY_EDITOR
                if(i == _debugSelectIndex)
                {
                    Debug.DrawRay(_nodes[i].Position, deltaPos * magnitude, Color.magenta);
                }

                if (i + 1 == _debugSelectIndex)
                {
                    Debug.DrawRay(_nodes[i + 1].Position, deltaPos * -magnitude, Color.magenta);
                }
#endif

                _nodes[i].Forward = deltaPos;
            }
            else
            {
                _nodes[i].Forward = _nodes[i - 1].Forward;
            }

            // Allow possible modifaction of node
            _modifyNodeAction?.Invoke(i, _nodes[i]);

            // Apply gravity to node
            if(UseGravityOverride && GravityOverride != Vector3.zero)
            {
                _nodes[i].ApplyForce(UseGravityOverride ? GravityOverride : Physics.gravity);
            }

            // Apply drag to node
            if (Drag > 0f)
            {
                // Drag equation: https://en.wikipedia.org/wiki/Drag_(physics)
                // Drag variable here is a combination of a few variables in actual equation:
                // Fluid density, cross sectional area, and drag coefficient.
                 _nodes[i].ApplyForce(_nodes[i].Velocity.normalized * -0.5f * Drag * _nodes[i].Velocity.sqrMagnitude * deltaTime);
            }
        }
    }

    public virtual Node GetNode(int index) => _nodes[index];

    public void SetModifyNodeAction(Action<int, Node> action) => _modifyNodeAction = action;
    public void ClearModifyNodeAction() => _modifyNodeAction = null;

    public Vector3[] GetNodePositions()
    {
        if (_nodes == null) return null;

        Vector3[] positions = new Vector3[_pointCount];
        for (int i = 0; i < _pointCount; ++i)
        {
            positions[i] = _nodes[i].Position;
        }

        return positions;
    }

#if UNITY_EDITOR
    public virtual void DrawGizmos()
    {
        if(_nodes != null)
        {
            for (int i = 0; i < _nodes.Length; ++i)
            {
                Gizmos.color = i == _debugSelectIndex ? Color.cyan : Color.blue;
                Gizmos.DrawWireSphere(_nodes[i].Position, 0.1f);
                Gizmos.DrawRay(_nodes[i].Position, _nodes[i].Forward);

                Gizmos.color = Color.green;
                Gizmos.DrawRay(_nodes[i].Position, _nodes[i].Velocity);

                UnityEditor.Handles.Label(_nodes[i].Position, i.ToString());
            }
        }
    }
#endif
}
