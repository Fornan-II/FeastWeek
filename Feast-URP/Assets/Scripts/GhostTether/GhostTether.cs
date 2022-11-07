using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTether : MonoBehaviour
{
    [SerializeField] private Chain mainChain;
    [SerializeField] private int chainVertexCount = 10;
    [SerializeField] private Transform tetherStart;
    [SerializeField] private Transform tetherEnd;
    [SerializeField] private float windTimeScale = 1f;
    [SerializeField] private float windSampleScale = 1f;
    [SerializeField] private Vector3 windVector = new Vector3(1, 1, 0.2f);
    [SerializeField] private LineRenderer lineRenderer;

    private bool _isBroken = false;

    public void BreakChain()
    {
        _isBroken = true;
        mainChain.FixedEndPosition = false;
        mainChain.RefreshNodeData();
        // Change to be some middle node, and tether breaks roughly in half.
    }

    private void Start()
    {
        mainChain.Initialize(tetherStart.position, tetherEnd.position, chainVertexCount);
        mainChain.SetModifyNodeAction(NodeModify);
        lineRenderer.positionCount = chainVertexCount;
        lineRenderer.SetPositions(mainChain.GetNodePositions());
    }

    private void FixedUpdate()
    {
        mainChain.ProcessPhysics(Time.fixedDeltaTime);
    }

    private void NodeModify(int i, Chain.Node n)
    {
        if (i == 0)
            n.Position = tetherStart.position;
        else if (i == mainChain.PointCount - 1 && !_isBroken)
            n.Position = tetherEnd.position;

        Vector3 windForce = windVector * Mathf.PerlinNoise(n.Position.z * windSampleScale, Time.timeSinceLevelLoad * windTimeScale);
        n.ApplyForce(windForce);

        lineRenderer.SetPosition(i, n.Position);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if(mainChain.Initialized)
            mainChain.DrawGizmos();
        else if(tetherStart && tetherEnd)
        {
            // Draw tether start and end points
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(tetherStart.position, 0.1f);
            Gizmos.DrawWireSphere(tetherEnd.position, 0.1f);

            // Draw vector
            if (windVector != Vector3.zero)
            {
                Gizmos.color = Color.green;
                Vector3 deltaPos = (tetherEnd.position - tetherStart.position) / mainChain.PointCount;
                for (int i = 0; i < mainChain.PointCount; ++i)
                {
                    Vector3 pos = tetherStart.position + deltaPos * i;
                    Gizmos.DrawRay(
                        pos,
                        windVector * Mathf.PerlinNoise(pos.z * windSampleScale, Time.timeSinceLevelLoad * windTimeScale)
                        );
                }
            }
        }
    }

    private void OnValidate()
    {
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
    }
#endif
}
