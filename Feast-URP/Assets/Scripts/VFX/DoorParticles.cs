using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private bool isPlaying;

    private void Start()
    {
        if (isPlaying)
            Activate();
    }

    public void Activate()
    {
        foreach (var p in particles)
            p.Play();
        isPlaying = true;
    }

    public void Deactivate()
    {
        foreach (var p in particles)
            p.Stop();
        isPlaying = false;
    }

    public void SensitiveActivate(float sensitivity)
    {
        if (isPlaying && sensitivity <= 0f)
            Deactivate();
        else if (!isPlaying && sensitivity > .5f)
            Activate();
    }

#if UNITY_EDITOR
    private bool _cachedPlaying;
    private void OnValidate()
    {
        if(UnityEditor.EditorApplication.isPlaying && isPlaying != _cachedPlaying)
        {
            if (isPlaying)
                Activate();
            else
                Deactivate();

            _cachedPlaying = isPlaying;
        }
    }
#endif
}
