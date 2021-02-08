using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Checkpoint LastCheckpoint => _lastCheckpoint;
#if UNITY_EDITOR
    public static Checkpoint DefaultCheckPoint
    {
        get
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                foreach (var cp in FindObjectsOfType<Checkpoint>())
                {
                    if (cp.isDefaultCheckpoint)
                        return cp;
                }
            }
            return _defaultCheckpoint;
        }
    }
#else
    public static Checkpoint DefaultCheckPoint => _defaultCheckpoint;
#endif

    private static Checkpoint _lastCheckpoint;
    private static Checkpoint _defaultCheckpoint;

    [SerializeField] private bool isDefaultCheckpoint = false;

    public static void ResetToCheckPoint(ICheckpointUser target)
    {
        MainCamera.Effects.CrossFade(0f, true);
        if (_lastCheckpoint)
            _lastCheckpoint.ResetAt(target);
        else
            _defaultCheckpoint.ResetAt(target);
        MainCamera.Effects.CrossFade(3f, false);
    }

    public void ResetAt(ICheckpointUser target) => target.Teleport(transform.position, transform.rotation);

    private void Awake()
    {
        if (isDefaultCheckpoint)
            _defaultCheckpoint = this;
    }

    private void OnDestroy()
    {
        if (_defaultCheckpoint == this)
            _defaultCheckpoint = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out ICheckpointUser cpu))
        {
            _lastCheckpoint = this;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(isDefaultCheckpoint)
        {
            foreach(var cp in FindObjectsOfType<Checkpoint>())
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
