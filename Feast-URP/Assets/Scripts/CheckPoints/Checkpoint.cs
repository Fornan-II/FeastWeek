using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private static Checkpoint _lastCheckpoint;
    private static Checkpoint _defaultCheckpoint;

    [SerializeField] private bool isDefaultCheckpoint = false;

    public static void ResetToCheckPoint(CheckpointUser target)
    {
        MainCamera.FadeScreen(0f, true);
        if (_lastCheckpoint)
            _lastCheckpoint.ResetAt(target);
        else
            _defaultCheckpoint.ResetAt(target);
        MainCamera.FadeScreen(3f);
    }

    public void ResetAt(CheckpointUser target)
    {
        target.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    private void Awake()
    {
        if (isDefaultCheckpoint)
            _defaultCheckpoint = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out CheckpointUser cpu))
        {
            _lastCheckpoint = this;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(isDefaultCheckpoint)
        {
            var allCP = FindObjectsOfType<Checkpoint>();
            foreach(var cp in allCP)
            {
                if (cp != this && cp.isDefaultCheckpoint)
                    cp.isDefaultCheckpoint = false;
            }

            if (UnityEditor.EditorApplication.isPlaying)
                _defaultCheckpoint = this;
        }
    }
#endif
}
