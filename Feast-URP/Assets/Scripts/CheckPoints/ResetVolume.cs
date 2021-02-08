using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ICheckpointUser cpu))
        {
            Checkpoint.ResetToCheckPoint(cpu);
        }
    }
}
