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
        public bool UseGravity = false;
        public float Drag = 0f;

        public Vector3 Velocity { get; private set; } = Vector3.zero;

        public void ProcessPhysics(float deltaTime)
        {
            if (!UsePhysics) return;

            if(Drag > 0f)
            {
                Velocity -= Util.LimitVector3(Velocity, Drag * deltaTime);
            }
            if(UseGravity)
            {
                Velocity += Physics.gravity * deltaTime;
            }

            Position += Velocity * deltaTime;
        }

        public void ApplyForce(Vector3 force)
        {
            if(UsePhysics)
                Velocity += force;
        }
    }

    public int PointCount => _pointCount;
    public bool Initialized => _nodes != null;
    
    [SerializeField] public bool fixedStartPosition = true;
    [SerializeField] public bool fixedEndPosition = true;
    [SerializeField, Min(0)] public float stiffness = 5;
    [SerializeField, Min(0)] public float lengthScaler = 1.2f;
    [SerializeField, Min(0)] public float drag = 0f;
    [SerializeField] public bool useGravity = true;

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
            if (i == 0) usePhysics = !fixedStartPosition;
            if (i == _nodes.Length - 1) usePhysics = !fixedEndPosition;


            _nodes[i] = new Node()
            {
                Position = startPosition + deltaPos * i,
                UsePhysics = usePhysics,
                UseGravity = useGravity,
                Drag = drag
            };
        }
    }

    public virtual void InitializeFrom(Chain other, int startIndex = 0, int lastIndex = -1)
    {
        // TODO: Copy function
    }

    public virtual void RefreshNodeData()
    {
        for(int i = 0; i < _nodes.Length; ++i)
        {
            bool usePhysics = true;
            if (i == 0) usePhysics = !fixedStartPosition;
            if (i == _nodes.Length - 1) usePhysics = !fixedEndPosition;

            _nodes[i].UsePhysics = usePhysics;
            _nodes[i].UseGravity = useGravity;
            _nodes[i].Drag = drag;
        }
    }

    public virtual void ProcessPhysics(float deltaTime)
    {
        // Run physics on chain nodes
        foreach (Node n in _nodes)
        {
            n.ProcessPhysics(deltaTime);
        }
        
        for (int i = 0; i < _nodes.Length; ++i)
        {
            // Apply chain forces to nodes
            // Checking the "chain" between the ith node and the node after
            if (i + 1 < _nodes.Length)
            {
                Vector3 deltaPos = _nodes[i + 1].Position - _nodes[i].Position;
                float magnitude = (deltaPos.magnitude - _targetLength * lengthScaler) * stiffness;

                _nodes[i].ApplyForce(deltaPos.normalized * magnitude);
                _nodes[i + 1].ApplyForce(deltaPos.normalized * -magnitude);
            }

            ModifyNodeAction?.Invoke(i, _nodes[i]);
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
