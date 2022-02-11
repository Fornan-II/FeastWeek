using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWorld : MonoBehaviour
{
    [SerializeField] private float fadeInDuration = 7f;

    private void Start()
    {
        MainCamera.Effects.SetFadeColor(Color.black);
        MainCamera.Effects.CrossFade(fadeInDuration, false);
    }
}
