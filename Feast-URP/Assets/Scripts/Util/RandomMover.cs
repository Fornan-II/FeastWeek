using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMover : MonoBehaviour
{
    [SerializeField] private Vector3 noiseSpeed = Vector3.one;
    [SerializeField] private float offsetScale = 1f;

    private Vector3 _seed;

    private void Start()
    {
        _seed = Random.insideUnitSphere * 1000f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = new Vector3(
            Mathf.PerlinNoise(_seed.x, Time.timeSinceLevelLoad * noiseSpeed.x),
            Mathf.PerlinNoise(_seed.y, Time.timeSinceLevelLoad * noiseSpeed.y),
            Mathf.PerlinNoise(_seed.z, Time.timeSinceLevelLoad * noiseSpeed.z)
            );

        transform.localPosition = offset * offsetScale;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.parent.localToWorldMatrix;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Vector3.zero, offsetScale);
    }
#endif
}
