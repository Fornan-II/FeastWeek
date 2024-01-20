using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakedAmbience : BakedBlendTriggerVolume
{
    #region Classes and Structs
    // These classes only relevant to the baking process. Before being used in game,
    // they are being converted down to a simpler struct type.
    public class BakeField3D : GenericField3D<BakeNode>
    {
        public BakeField3D() : base() { }
        public BakeField3D(int xSize, int ySize, int zSize) : base(xSize, ySize, zSize) { }
    }

    public struct BakeNode
    {
        public int SourcesVisible;
        public int Bounce;
    }
    #endregion
    
    [Header("Baking")]
    [SerializeField] private Transform[] soundSources;
    [SerializeField] private LayerMask bakeMask = Physics.AllLayers;

    

    private bool HasLineOfSight(Vector3Int fromNode, Vector3Int toNode)
    {
        Vector3 worldFrom = transform.TransformPoint(GetLocalPositionFromIndex(fromNode));
        Vector3 worldTo = transform.TransformPoint(GetLocalPositionFromIndex(toNode));
        Vector3 dir = worldTo - worldFrom;
        return !Physics.Raycast(worldFrom, dir, dir.magnitude, bakeMask, QueryTriggerInteraction.Ignore);
    }

#if UNITY_EDITOR
    private void EDITOR_CheckAllAgainstSources(ref BakeField3D probeField, List<Vector3Int> all, List<Vector3Int> sources)
    {
        foreach(Vector3Int sourceIndex in sources)
        {
            foreach(Vector3Int index in all)
            {
                if(HasLineOfSight(sourceIndex, index))
                {
                    BakeNode thisNode = probeField.GetValue(index.x, index.y, index.z);
                    BakeNode sourceNode = probeField.GetValue(sourceIndex.x, sourceIndex.y, sourceIndex.z);
                    thisNode.Bounce = sourceNode.Bounce + 1;
                    thisNode.SourcesVisible += sourceNode.SourcesVisible; // Think about this? !! Most nodes have 0! Non-source though?
                    probeField.SetValue(index.x, index.y, index.z, thisNode);
                }
            }
        }

        // Recursive ability where all is nodes that don't have line of sight,
        // and sources are all edge nodes of this iteration?
        // Recursion happens until some MaxBounce variable?
    }

    [ContextMenu("Bake ambience occlusion")]
    private void EDITOR_BakeAmbience()
    {
        // Initialize BakeField
        BakeField3D probeField = new BakeField3D(probeCount.x, probeCount.y, probeCount.z);
        List<Vector3Int> unvisitedNodes = new List<Vector3Int>();
        for(int x = 0; x < probeCount.x; ++x)
        {
            for(int y = 0; y < probeCount.y; ++y)
            {
                for(int z = 0; z < probeCount.z; ++z)
                {
                    unvisitedNodes.Add(new Vector3Int(x, y, z));
                    // Bounce -1 doesn't really matter... can be treated as 0.
                    // Technically, it does indicate that it is an initial source.
                    // SourcesVisible 0 however means it's an invalid node.
                    probeField.SetValue(x, y, z, new BakeNode() { Bounce = 0, SourcesVisible = 0 });
                }
            }
        }

        // Represent audio sources in the field
        List<Vector3Int> nodeSources = new List<Vector3Int>();
        foreach(var t in soundSources)
        {
            Vector3Int index = GetNearestIndexFromLocalPosition(transform.InverseTransformPoint(t.position));
            if(nodeSources.Contains(index))
            {
                BakeNode n = probeField.GetValue(index.x, index.y, index.z);
                ++n.SourcesVisible;
                probeField.SetValue(index.x, index.y, index.z, n);
            }
            else
            {
                nodeSources.Add(index);
                unvisitedNodes.Remove(index);
                probeField.SetValue(index.x, index.y, index.z, new BakeNode() { Bounce = -1, SourcesVisible = 1 });
            }
        }

        // Bake occlusion
        EDITOR_CheckAllAgainstSources(ref probeField, unvisitedNodes, nodeSources);

        // For now... translate BakeField to a FloatField for realtime use?
        // And in that realtime use... have value affect filter, not volume.
        // Or at least, filter much more than volume.
        _probeField = new FloatField3D(probeCount.x, probeCount.y, probeCount.z);

        for (int x = 0; x < probeCount.x; ++x)
        {
            for (int y = 0; y < probeCount.y; ++y)
            {
                for (int z = 0; z < probeCount.z; ++z)
                {
                    BakeNode node = probeField.GetValue(x, y, z);

                    // 1 is no occlusion, zero is full occlusion
                    float value;
                    if (node.Bounce == -1) // Source node
                    {
                        value = 1f;
                    }
                    else
                    {
                        value = Mathf.Clamp01(node.SourcesVisible / Mathf.Min(soundSources.Length, 7f));
                    }
                    
                    _probeField.SetValue(x, y, z, value);
                }
            }
        }
    }
#endif
}
