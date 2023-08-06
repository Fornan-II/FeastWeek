using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerListener
{
    void OnOverlapStart();
    void OnOverlap();
    void OnOverlapExit();
}
