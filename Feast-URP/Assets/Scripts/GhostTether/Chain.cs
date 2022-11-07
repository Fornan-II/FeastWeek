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
    
    public bool FixedStartPosition = true;
    public bool FixedEndPosition = true;
    [Min(0)] public float stiffness = 5;
    [Min(0)] public float lengthScaler = 1.2f;
    [Min(0), Tooltip("Product of fluid density, cross sectional area, and drag coefficient.")] public float Drag = 0f;
    public bool UseGravityOverride = false;
    public Vector3 GravityOverride = Vector3.zero;

    protected int _pointCount;
    protected Node[] _nodes;
    protected float _targetLength;
    
    protected Action<int, Node> ModifyNodeAction;
    
    public virtual void Initialize(Vector3 startPosition, Vector3 endPosition, int pointCount)
    {
        _pointCount = pointCount;
        _nodes = new Node[_pointCount];

        Vector3 deltaPos = endPosition - startPosition;
        _targetLength = deltaPos.magnitude / _pointCount;
        deltaPos = deltaPos.normalized * _targetLength;

        // The same as RefreshNodeData, except for also setting position
        for (int i = 0; i < _nodes.Length; ++i)
        {
            bool usePhysics = true;
            if (i == 0) usePhysics = !FixedStartPosition;
            if (i == _nodes.Length - 1) usePhysics = !FixedEndPosition;


            _nodes[i] = new Node()
            {
                Position = startPosition + deltaPos * i,
                UsePhysics = usePhysics
            };
        }
    }

    public virtual void InitializeFrom(Chain other, int startIndex = 0, int lastIndex = -1)
    {
        if(lastIndex < 0)
        {
            lastIndex = other.PointCount - 1;
        }

        _pointCount = lastIndex - startIndex + 1;
        _nodes = new Node[_pointCount];

        for(int i = 0; i < _nodes.Length; ++i)
        {
            _nodes[i] = new Node(other._nodes[i + startIndex]);
        }
    }

    public virtual void RefreshNodeData()
    {
        for(int i = 0; i < _nodes.Length; ++i)
        {
            bool usePhysics = true;
            if (i == 0) usePhysics = !FixedStartPosition;
            if (i == _nodes.Length - 1) usePhysics = !FixedEndPosition;

            _nodes[i].UsePhysics = usePhysics;
        }
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

                _nodes[i].ApplyForce(deltaPos.normalized * magnitude);
                _nodes[i + 1].ApplyForce(deltaPos.normalized * -magnitude);
            }

            // Allow possible modifaction of node
            ModifyNodeAction?.Invoke(i, _nodes[i]);

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

    public void SetModifyNodeAction(Action<int, Node> action) => ModifyNodeAction = action;
    public void ClearModifyNodeAction() => ModifyNodeAction = null;

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
            Gizmos.color = Color.blue;
            foreach (Node n in _nodes)
            {
                Gizmos.DrawWireSphere(n.Position, 0.1f);
            }
        }
    }
#endif
}
