using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWorldHelper : MonoBehaviour
{
    [SerializeField] private GhostWorld ghostWorldScript;

    public void EndSequence3Done() => ghostWorldScript.EndEndAnimation();
}
