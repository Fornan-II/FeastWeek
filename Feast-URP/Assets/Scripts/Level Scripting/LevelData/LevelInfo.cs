using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    public static List<LevelInfo> Active { get; private set; } = new List<LevelInfo>(1);

    [SerializeField] private FogData fogData;

    private void Awake()
    {
        Active.Add(this);

        fogData?.Apply();
    }

    private void OnDestroy()
    {
        Active.Remove(this);
    }
}
