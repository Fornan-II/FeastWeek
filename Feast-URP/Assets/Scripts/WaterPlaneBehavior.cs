using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlaneBehavior : MonoBehaviour
{
    private float _yPosition;

    private void Start() => _yPosition = transform.position.y;

    private void Update()
    {
        if (!MainCamera.Instance) return;

        Vector3 position = MainCamera.Instance.transform.position;
        position.y = _yPosition;
        transform.position = position;
    }
}
