using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTether : Chain
{
    [SerializeField] private Transform tetherStart;
    [SerializeField] private Transform tetherEnd;
    [SerializeField] private float windTimeScale = 1f;
    [SerializeField] private float windSampleScale = 1f;
    [SerializeField] private Vector3 windVector = new Vector3(1, 1, 0.2f);

    private bool _isBroken = false;

    public void BreakChain()
    {
        _isBroken = true;
        nodes[nodes.Length - 1].UsePhysics = true;
        // Change to be some middle node, and tether breaks roughly in half.
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        nodes[0].Position = tetherStart.position;
        if(!_isBroken)
            nodes[nodes.Length - 1].Position = tetherEnd.position;
    }

    protected override void NodeModify(Node n)
    {
        Vector3 windForce = windVector * Mathf.PerlinNoise(n.Position.z * windSampleScale, Time.timeSinceLevelLoad * windTimeScale);
        n.ApplyForce(windForce);
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, windVector);
    }

    private void OnValidate()
    {
        if(tetherStart)
        {
            startingPosition = tetherStart.position;
        }

        if(tetherEnd)
        {
            endingPosition = tetherEnd.position;
        }
    }
#endif
}
