using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://twitter.com/HackTrout/status/1362008498444320770?s=20&t=i2afNUCHo24IFw0zLDmbdw

public class Chain : MonoBehaviour
{
    [System.Serializable]
    class Node
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

    [Header("Chain Physics")]
    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private Vector3 endingPosition;
    [SerializeField, Min(0)] private int pointCount = 4;
    [SerializeField, Min(0)] private float stiffness = 5;
    [SerializeField, Min(0)] private float lengthScaler = 1.2f;
    [SerializeField, Min(0)] private float drag = 0f;
    [SerializeField] private bool useGravity = true;
    [Header("Visuals")]
    [SerializeField] private LineRenderer lineRenderer;

    private Node[] nodes;
    private float targetLength;

    private void Start()
    {
        nodes = new Node[pointCount];
        // Init renderer point count
        lineRenderer.positionCount = nodes.Length;

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

            // Set renderer point count
            lineRenderer.SetPosition(i, nodes[i].Position);
        }
    }

    private void FixedUpdate()
    {
        foreach (Node n in nodes)
        {
            n.ProcessPhysics(Time.fixedDeltaTime);
        }

        for (int i = 0; i < nodes.Length; ++i)
        {
            // Checking the "chain" between the ith node and the node after
            if (i + 1 < nodes.Length)
            {
                Vector3 deltaPos = nodes[i + 1].Position - nodes[i].Position;
                float magnitude = (deltaPos.magnitude - targetLength * lengthScaler) * stiffness;

                nodes[i].ApplyForce(deltaPos.normalized * magnitude);
                nodes[i + 1].ApplyForce(deltaPos.normalized * -magnitude);
            }

            // Update renderer position
            lineRenderer.SetPosition(i, nodes[i].Position);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
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
