using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Footstep Data", menuName = "Data/New Footstep Data")]
public class FootstepData : ScriptableObject
{
    // Improvement: FootstepDataPair matches AudioType to both SurfaceType, and to foot type.
    //              Not necessary for City by the Seaa since there's only one foot type.

    [System.Serializable]
    private struct FootstepDataPair
    {
        public FootstepSurface.SurfaceType SurfaceType;
        public AudioClip FootstepSound;
    }

    [SerializeField] private FootstepDataPair[] surfaceTypeAudioClips;

    public Dictionary<FootstepSurface.SurfaceType, AudioClip> GetSurfaceTypeAudioClips()
    {
        Dictionary<FootstepSurface.SurfaceType, AudioClip> data = new Dictionary<FootstepSurface.SurfaceType, AudioClip>();
        foreach(FootstepDataPair item in surfaceTypeAudioClips)
        {
            data.Add(item.SurfaceType, item.FootstepSound);
        }
        return data;
    }
}
