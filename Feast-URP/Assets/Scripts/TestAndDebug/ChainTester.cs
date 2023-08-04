using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainTester : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GhostTether tether;

    private void Update()
    {
        if (!tether) return;

        Chain.Node node = tether.GetNearestChainNode(transform.position);
        if(node != null)
        {
            Debug.DrawLine(node.Position, transform.position, Color.cyan);
        }
    }
}
