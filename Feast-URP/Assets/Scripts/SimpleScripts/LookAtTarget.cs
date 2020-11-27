using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform Target;

    // Update is called once per frame
    void Update()
    {
        if(Target)
        {
            transform.LookAt(Target, Vector3.up);
        }
    }
}
