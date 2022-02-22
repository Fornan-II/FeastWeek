using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLookAtTarget : MonoBehaviour
{
    public Transform Target;

    // Update is called once per frame
    void Update()
    {
        if(Target)
        {
            Vector3 vecToTarget = Target.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(vecToTarget, Vector3.up);
        }
    }
}
