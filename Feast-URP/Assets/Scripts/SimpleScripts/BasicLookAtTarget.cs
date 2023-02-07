using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLookAtTarget : MonoBehaviour
{
    public Transform Target;
    public Vector3 WorldSpaceOffset;

    // Update is called once per frame
    void Update()
    {
        if(Target)
        {
            Vector3 vecToTarget = Target.transform.position - transform.position + WorldSpaceOffset;
            transform.rotation = Quaternion.LookRotation(vecToTarget, Vector3.up);
        }
    }
}
