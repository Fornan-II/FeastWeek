using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICheckpointUser
{
    void Teleport(Vector3 position, Quaternion rotation);
}
