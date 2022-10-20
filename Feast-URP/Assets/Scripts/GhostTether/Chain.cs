using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// https://twitter.com/HackTrout/status/1362008498444320770?s=20&t=i2afNUCHo24IFw0zLDmbdw

public class Chain : MonoBehaviour
{
    [System.Serializable]
    protected class Node
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

        public void ApplyForce(Vector3 force) => Velocity += force;
    }

    public int PointCount => pointCount;

    [Header("Chain Physics")]
    [SerializeField] protected Vector3 startingPosition;
    [SerializeField] protected Vector3 endingPosition;
    [SerializeField, Min(0)] protected int pointCount = 4;
    [SerializeField, Min(0)] protected float stiffness = 5;
    [SerializeField, Min(0)] protected float lengthScaler = 1.2f;
    [SerializeField, Min(0)] protected float drag = 0f;
    [SerializeField] protected bool useGravity = true;

    protected Node[] nodes;
    protected float targetLength;
    
    protected event UnityAction<int, Vector3> OnNodePositionUpdate;

    public void AddNodePositionListener(UnityAction<int, Vector3> listener) => OnNodePositionUpdate += listener;
    public void RemoveNodePositionListener(UnityAction<int, Vector3> listener) => OnNodePositionUpdate -= listener;

    public Vector3[] GetNodePositions()
    {
        if (nodes == null) return null;

        Vector3[] positions = new Vector3[pointCount];
        for(int i = 0; i < pointCount; ++i)
        {
            positions[i] = nodes[i].Position;
        }

        return positions;
    }

    #region Unity Methods
    protected virtual void Awake()
    {
        nodes = new Node[pointCount];

        Vector3 deltaPos = endingPosition - startingPosition;
        targetLength = deltaPos.magnitude / pointCount;
        deltaPos = deltaPos.normalized * targetLength;

        for (int i = 0; i < nodes.Length; ++i)
        {
            nodes[i] = new Node()
            {
                Position = startingPosition + deltaPos * i,
                UsePhysics = i != 0 && i != pointCount - 1,
                UseGravity = useGravity,
                Drag = drag
            };
        }
    }

    protected virtual void FixedUpdate()
    {
        // Run physics on chain nodes
        foreach (Node n in nodes)
        {
            n.ProcessPhysics(Time.fixedDeltaTime);
        }
        
        for (int i = 0; i < nodes.Length; ++i)
        {
            // Apply chain forces to nodes
            // Checking the "chain" between the ith node and the node after
            if (i + 1 < nodes.Length)
            {
                Vector3 deltaPos = nodes[i + 1].Position - nodes[i].Position;
                float magnitude = (deltaPos.magnitude - targetLength * lengthScaler) * stiffness;

                nodes[i].ApplyForce(deltaPos.normalized * magnitude);
                nodes[i + 1].ApplyForce(deltaPos.normalized * -magnitude);
            }

            NodeModify(nodes[i]);

            // Update renderer position
            OnNodePositionUpdate(i, nodes[i].Position);
        }
    }
    #endregion

    protected virtual void NodeModify(Node n) { }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        if(nodes != null)
        {
            Gizmos.color = Color.blue;
            foreach (Node n in nodes)
            {
                Gizmos.DrawWireSphere(n.Position, 0.1f);
            }
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(startingPosition, 0.1f);
            Gizmos.DrawWireSphere(endingPosition, 0.1f);
        }
    }
#endif
}
