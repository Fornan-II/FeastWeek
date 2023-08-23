using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GhostTetherBaker : MonoBehaviour
{
#if UNITY_EDITOR
#pragma warning disable 0649
    private const string BakedTetherNodeTransformParentName = "Baked Tether Nodes";
    [System.Serializable]
    private struct BounceNode
    {
        public Transform Transform;
        public float Multiplier;
    }

    [SerializeField] private bool showGizmos = true;
    [SerializeField] private BakedBlendTriggerVolume bakeTarget;
    [SerializeField] private GhostTether[] tethers;
    [SerializeField] private BounceNode[] bounceNodes;
    [SerializeField] private LayerMask bakeLayerMask = Physics.AllLayers;
    
    [ContextMenu("Create baked tether node transforms")]
    private void CreateBakedTetherNodeTransforms()
    {
        Undo.RecordObject(this, "Create baked tether node transforms");

        if (tethers.Length <= 0)
        {
            Debug.LogWarning("Tethers has no assigned members! No tether node transforms will be baked.");
        }

        Transform bakedParent = transform.Find(BakedTetherNodeTransformParentName);
        if(!bakedParent)
        {
            bakedParent = new GameObject(BakedTetherNodeTransformParentName).transform;
            Util.MoveTransformToTarget(bakedParent, transform, true);
            Undo.RecordObject(bakedParent, "Create baked tether node transforms");
        }
        else
        {
            while(bakedParent.childCount > 0)
            {
                Undo.RecordObject(bakedParent.GetChild(0), "Create baked tether node transforms");
                Destroy(bakedParent.GetChild(0));
            }
        }

        int counter = 0;
        foreach(GhostTether tether in tethers)
        {
            foreach(Vector3 pos in tether.GetTetherNodePositions())
            {
                Transform bakedTetherNode = new GameObject(string.Format("Baked Tether Node {0}", counter)).transform;
                ++counter;
                bakedTetherNode.position = pos;
                bakedTetherNode.parent = bakedParent;
                Undo.RecordObject(bakedTetherNode, "Create baked tether node transforms");
            }
        }
    }

    [ContextMenu("Start baking GhostTether Occlusion")]
    private void Bake()
    {
        if(!bakeTarget)
        {
            Debug.LogError("Bake aborted: bakeTarget can not be null!");
            return;
        }

        Transform bakedSources = transform.Find(BakedTetherNodeTransformParentName);

        if(!bakedSources)
        {
            Debug.LogErrorFormat("Bake aborted: could not find child GameObject with with name '{0}'! Make sure to set up baked tether nodes.", BakedTetherNodeTransformParentName);
            return;
        }

        Undo.RecordObject(bakeTarget, "Bake GhostTether Occlusion");

        bakeTarget.EDITOR_GenerateProbeField(
            (Vector3 probePosition) =>
            {
                for (int i = 0; i < bakedSources.childCount; ++i)
                {
                    Transform source = bakedSources.GetChild(i);
                    if(!Physics.Linecast(source.position, probePosition, bakeLayerMask, QueryTriggerInteraction.Ignore))
                    {
                        return 0f; // No occlusion
                    }
                }

                float value = 1f; // Fully occluded

                foreach(var node in bounceNodes)
                {
                    if(node.Transform && !Physics.Linecast(node.Transform.position, probePosition, bakeLayerMask, QueryTriggerInteraction.Ignore))
                    {
                        value = Mathf.Min(value, node.Multiplier); // Take least amount of occlusion
                    }
                }

                return value;
            } );
        
        Debug.Log("Bake completed successfully");
    }

    private void OnDrawGizmos()
    {
        // Fun Fact: IsChildOf returns true when checking a transform against itself.
        if (Selection.activeTransform && Selection.activeTransform != transform && Selection.activeTransform.IsChildOf(transform))
        {
            OnDrawGizmosSelected();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Transform bakedSources = transform.Find(BakedTetherNodeTransformParentName);
        if (bakedSources && bakedSources.childCount > 0)
        {
            for (int i = 0; i < bakedSources.childCount; ++i)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(bakedSources.GetChild(i).position, 0.5f);
                foreach (var node in bounceNodes)
                {
                    if (node.Transform)
                    {
                        if (i == 0)
                        {
                            Gizmos.color = Color.cyan;
                            Gizmos.DrawWireSphere(node.Transform.position, 0.1f);
                        }
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(bakedSources.GetChild(i).position, node.Transform.position);
                    }
                }
            }
        }
        else
        {
            Gizmos.color = Color.cyan;
            foreach (var node in bounceNodes)
            {
                if(node.Transform)
                    Gizmos.DrawWireSphere(node.Transform.position, 0.1f);
            }
        }
    }
#endif
}
