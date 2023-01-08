using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTether : MonoBehaviour
{
    [System.Serializable]
    private struct FixedChainPoint
    {
        public int Index;
        public Transform Point;
    }

    [Header("Chain Settings")]
    [SerializeField] private Chain mainChain;
    [SerializeField] private int chainVertexCount = 10;
    [SerializeField, Tooltip("The indices of this are expected to be in order from low to high. No, I will not be writing code to do this automatically.")]
    private FixedChainPoint[] fixedPoints;
    [Header("Wind Settings")]
    [SerializeField] private float windTimeScale = 1f;
    [SerializeField] private float windSampleScale = 1f;
    [SerializeField] private Vector3 windVector = new Vector3(1, 1, 0.2f);
    [Header("Break Settings")]
    [SerializeField] private float breakForce = 3f;
    [Header("Visuals")]
    [SerializeField] private LineRenderer lineRendererMain;
    [SerializeField] private LineRenderer lineRendererSecondary;

    private bool _isBroken = false;
    private Chain _secondaryChain;
    
    public void BreakChain()
    {
        // Cut chain in half
        int cutIndex = GetCutIndex();
        _secondaryChain = new Chain(mainChain, cutIndex);
        mainChain = new Chain(mainChain, 0, cutIndex - 1);
        _secondaryChain.SetModifyNodeAction(NodeModify_Secondary);

        // Change stiffness and length scalar?

        // Apply some physics for dramatic effect
        Chain.Node nodeA = mainChain.GetNode(mainChain.PointCount - 1);
        Chain.Node nodeB = _secondaryChain.GetNode(0);
        Vector3 forceVector = (nodeB.Position - nodeA.Position).normalized * breakForce;
        nodeA.ApplyForce(forceVector * -1);
        nodeB.ApplyForce(forceVector);

        // Update visuals
        lineRendererMain.positionCount = mainChain.PointCount;
        lineRendererMain.SetPositions(mainChain.GetNodePositions());
        lineRendererSecondary.enabled = true;
        lineRendererSecondary.positionCount = _secondaryChain.PointCount;
        lineRendererSecondary.SetPositions(_secondaryChain.GetNodePositions());

        // Finalize
        _isBroken = true;
    }

    private void Start()
    {
        if(fixedPoints.Length < 2)
        {
            Debug.LogError("GhostTether failed to generate chain: fixedPoints length is less than 2.");
            return;
        }

        // Init chain
        mainChain.Initialize(fixedPoints[0].Point.position, fixedPoints[fixedPoints.Length - 1].Point.position, chainVertexCount);
        mainChain.SetModifyNodeAction(NodeModify_Main);

        // Init visuals
        lineRendererMain.positionCount = chainVertexCount;
        lineRendererMain.SetPositions(mainChain.GetNodePositions());
        lineRendererSecondary.enabled = false;
    }

    private void FixedUpdate()
    {
        mainChain.ProcessPhysics(Time.fixedDeltaTime);
        if (_isBroken)
        {
            _secondaryChain.ProcessPhysics(Time.fixedDeltaTime);
        }

        foreach(var fixedPoint in fixedPoints)
        {
            int cutIndex = GetCutIndex();

            if(_isBroken && fixedPoint.Index >= cutIndex)
            {
                _secondaryChain.GetNode(fixedPoint.Index - cutIndex).Position = fixedPoint.Point.position;
            }
            else
            {
                mainChain.GetNode(fixedPoint.Index).Position = fixedPoint.Point.position;
            }
        }
    }

    private int GetCutIndex() => chainVertexCount / 2;

    private void NodeModify_Main(int i, Chain.Node n)
    {
        Vector3 windForce = windVector * Mathf.PerlinNoise(n.Position.z * windSampleScale, Time.timeSinceLevelLoad * windTimeScale);
        n.ApplyForce(windForce);

        lineRendererMain.SetPosition(i, n.Position);
    }

    private void NodeModify_Secondary(int i, Chain.Node n)
    {
        Vector3 windForce = windVector * Mathf.PerlinNoise(n.Position.z * windSampleScale, Time.timeSinceLevelLoad * windTimeScale);
        n.ApplyForce(windForce);

        lineRendererSecondary.SetPosition(i, n.Position);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if(mainChain.Initialized)
        {
            mainChain.DrawGizmos();

            if(_isBroken)
            {
                _secondaryChain.DrawGizmos();
            }
        }
        else if(fixedPoints.Length >= 2)
        {
            // Draw tether start and end points
            Gizmos.color = Color.yellow;
            foreach (var fixedPoint in fixedPoints)
            {
                if(fixedPoint.Point)
                    Gizmos.DrawWireSphere(fixedPoint.Point.position, 0.1f);
            }
        }
    }

    [ContextMenu("Break chain")]
    private void EditorBreakChain()
    {
        if (!UnityEditor.EditorApplication.isPlaying) return;
        BreakChain();
    }
#endif
}
