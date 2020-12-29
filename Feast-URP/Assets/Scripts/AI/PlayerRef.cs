using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRef : MonoBehaviour
{
    public static Transform Transform { get; private set; }

    private void OnEnable() => Transform = transform;
    private void OnDisable() => Transform = null;
}
