using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLineRenderer : MonoBehaviour
{
    [SerializeField] private Chain chain;
    [SerializeField] private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.positionCount = chain.PointCount;
        lineRenderer.SetPositions(chain.GetNodePositions());
        chain.AddNodePositionListener(lineRenderer.SetPosition);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!chain) chain = GetComponent<Chain>();
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
    }
#endif
}
