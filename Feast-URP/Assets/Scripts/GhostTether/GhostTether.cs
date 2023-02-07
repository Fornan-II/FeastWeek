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
    [SerializeField, Tooltip("The indices of this are expected to be in order from low to high. No, I will not be writing code to do this automatically.")]
    private FixedChainPoint[] fixedPoints;
    [Header("Wind Settings")]
    [SerializeField] private float windTimeScale = 1f;
    [SerializeField] private float windSampleScale = 1f;
    [SerializeField] private Vector3 windVector = new Vector3(1, 1, 0.2f);
    [SerializeField] private Vector2Int interiorOverrideIndexRange;
    [Header("Break Settings")]
    [SerializeField] private int breakIndex;
    [SerializeField] private float breakForce = 3f;
    [SerializeField] private float breakDrag = 5f;
    [SerializeField] private float breakLengthScaler = 1f;
    [SerializeField] private AnimationCurve breakDissolve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve treeDissolve = AnimationCurve.Linear(0, -0.01f, 1, 0.95f);
    [SerializeField] private float tetherBreakCompleteValue = 1.0f;
    [Header("Visuals")]
    [SerializeField] private Transform[] tetherBones;
    [SerializeField] private Renderer tetherRenderer;
    [SerializeField] private Renderer treeRenderer;

    private bool _isBroken = false;
    private Chain _secondaryChain;
    private event System.Action _OnTetherDissolveComplete;

    public void BreakChain()
    {
        // Change chain properties for effect
        mainChain.Drag = breakDrag;
        mainChain.lengthScaler = breakLengthScaler;

        // Cut chain in half
        _secondaryChain = new Chain(mainChain, breakIndex);
        mainChain = new Chain(mainChain, 0, breakIndex - 1);
        _secondaryChain.SetModifyNodeAction(NodeModify_Secondary);

        // Apply some physics for dramatic effect
        Chain.Node nodeA = mainChain.GetNode(mainChain.PointCount - 1);
        Chain.Node nodeB = _secondaryChain.GetNode(0);
        Vector3 forceVector = (nodeB.Position - nodeA.Position).normalized * breakForce;
        nodeA.ApplyForce(forceVector * -1);
        nodeB.ApplyForce(forceVector);

        // Update visuals
        StartCoroutine(BreakAnimation());

        // Finalize
        _isBroken = true;
    }

    public void AddTetherDissolveCompleteListener(System.Action listener) => _OnTetherDissolveComplete += listener;
    public void RemoveTetherDissolveCompleteListener(System.Action listener) => _OnTetherDissolveComplete = listener;

    private void Start()
    {
        if(fixedPoints.Length < 2)
        {
            Debug.LogError("GhostTether failed to generate chain: fixedPoints length is less than 2.");
            return;
        }

        // Init chain
        // We are assuming that fixedPoints is a sorted array, by index.
        mainChain.Initialize(tetherBones.Length, GenerateChainNode);
        mainChain.SetModifyNodeAction(NodeModify_Main);

        // Init trees
        treeRenderer.sharedMaterial.SetFloat("_TreeStepEdge", treeDissolve.Evaluate(0f));
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
            if(_isBroken && fixedPoint.Index >= breakIndex)
            {
                _secondaryChain.GetNode(fixedPoint.Index - breakIndex).Position = fixedPoint.Point.position;
            }
            else
            {
                mainChain.GetNode(fixedPoint.Index).Position = fixedPoint.Point.position;
            }
        }
    }

    private void GenerateChainNode(Chain.Node[] nodes)
    {
        // Assuming that there's always at least two fixed points
        int nextFixedPoint = 1;

        Vector3 segmentStart = fixedPoints[nextFixedPoint - 1].Point.position;
        Vector3 segmentEnd = fixedPoints[nextFixedPoint].Point.position;
        Vector3 deltaPos = (segmentEnd - segmentStart) / (fixedPoints[nextFixedPoint].Index - fixedPoints[nextFixedPoint - 1].Index);

        for (int i = 0; i < nodes.Length; ++i)
        {
            nodes[i] = new Chain.Node(segmentStart + deltaPos * (i - fixedPoints[nextFixedPoint - 1].Index));

            // Handle if we are at the end of this segment between fixed points
            if (i == fixedPoints[nextFixedPoint].Index)
            {
                nodes[i].UsePhysics = false;

                ++nextFixedPoint;
                if(nextFixedPoint < fixedPoints.Length)
                {
                    segmentStart = fixedPoints[nextFixedPoint - 1].Point.position;
                    segmentEnd = fixedPoints[nextFixedPoint].Point.position;
                    deltaPos = (segmentEnd - segmentStart) / (fixedPoints[nextFixedPoint].Index - fixedPoints[nextFixedPoint - 1].Index);
                }
            }
        }

        // Fix UsePhysics not being set correctly for first fixed point
        nodes[fixedPoints[0].Index].UsePhysics = false;
    }

    private void NodeModify_Main(int i, Chain.Node n)
    {
        Vector3 windForce = windVector * Mathf.PerlinNoise(n.Position.z * windSampleScale, Time.timeSinceLevelLoad * windTimeScale);
        n.ApplyForce(windForce);

        if(interiorOverrideIndexRange.x <= i && i < interiorOverrideIndexRange.y)
        {
            n.ApplyForce(-mainChain.GravityOverride + new Vector3(0, -windVector.y * 0.5f, 0));
        }

        //lineRendererMain.SetPosition(i, n.Position);
        tetherBones[i].position = n.Position;
        tetherBones[i].up = n.Forward; // Using up instead of forward because that's how I made it on the mesh and I'm not fixing it
    }

    private void NodeModify_Secondary(int i, Chain.Node n)
    {
        i += breakIndex;

        Vector3 windForce = windVector * Mathf.PerlinNoise(n.Position.z * windSampleScale, Time.timeSinceLevelLoad * windTimeScale);
        n.ApplyForce(windForce);

        if (interiorOverrideIndexRange.x <= i && i < interiorOverrideIndexRange.y)
        {
            n.ApplyForce(-mainChain.GravityOverride + new Vector3(0, -windVector.y * 0.5f, 0));
        }

        tetherBones[i].position = n.Position;
        tetherBones[i].up = n.Forward; // Using up instead of forward because that's how I made it on the mesh and I'm not fixing it
    }

    private IEnumerator BreakAnimation()
    {
        Material tetherMat = tetherRenderer.material;
        Material treeMat = treeRenderer.material;
        int tetherBreakID = Shader.PropertyToID("_TetherBreak");
        int treeEdgeID = Shader.PropertyToID("_TreeStepEdge");
        bool hasBreakNotified = false;

        float duration = Mathf.Max(Util.AnimationCurveLengthTime(breakDissolve), Util.AnimationCurveLengthTime(treeDissolve));

        for(float timer = 0.0f; timer < duration; timer += Time.deltaTime)
        {
            float tBreak = breakDissolve.Evaluate(timer);
            tetherMat.SetFloat(tetherBreakID, tBreak);
            treeMat.SetFloat(treeEdgeID, treeDissolve.Evaluate(timer));

            if(!hasBreakNotified && tBreak >= tetherBreakCompleteValue)
            {
                // Some discrepency over timing of this events usage with changing design needs.
                _OnTetherDissolveComplete?.Invoke();
                hasBreakNotified = true;
            }

            yield return null;
        }

        if(!hasBreakNotified)
        {
            _OnTetherDissolveComplete?.Invoke();
        }
        
        // Disable this object when animation complete, as gameobject will be invisible and no longer needed.
        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if(!UnityEditor.EditorApplication.isPlaying)
        {
            for (int i = 0; i < tetherBones.Length; ++i)
            {
                UnityEditor.Handles.Label(tetherBones[i].position, i.ToString());
            }

            // Draw tether fixed points
            Gizmos.color = Color.yellow;
            foreach (var fixedPoint in fixedPoints)
            {
                if (fixedPoint.Point)
                    Gizmos.DrawWireSphere(fixedPoint.Point.position, 0.1f);
            }
        }
        else if (mainChain.Initialized)
        {
            mainChain.DrawGizmos();

            if (_isBroken)
            {
                _secondaryChain.DrawGizmos();
            }
        }
    }

    [ContextMenu("Break chain")]
    private void EditorBreakChain()
    {
        if (!UnityEditor.EditorApplication.isPlaying) return;
        BreakChain();
    }

    [ContextMenu("Sort Fixed Points array")]
    private void SortFixedPoints()
    {
        UnityEditor.Undo.RecordObject(this, "Sort fixed points array");
        for(int i = 0; i < fixedPoints.Length; ++i)
        {
            for(int j = i - 1; j >= 0 && fixedPoints[j].Index > fixedPoints[j + 1].Index; --j)
            {
                FixedChainPoint temp = fixedPoints[j];
                fixedPoints[j] = fixedPoints[j + 1];
                fixedPoints[j + 1] = temp;
            }
        }
    }
#endif
}
