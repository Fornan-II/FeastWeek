using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LookAtTarget : MonoBehaviour
{
#pragma warning disable 0649
    public Transform Target;
    public MultiAimConstraint Constraint => _constraint;

    [SerializeField] private MultiAimConstraint _constraint;

    [SerializeField] private Vector2 dotRange = new Vector2(-0.1f, 0.1f);

    // Update is called once per frame
    void Update()
    {
        if(Target)
        {
            transform.position = Target.position;
        }

        if(_constraint)
        {
            // Make the constrain ineffective when target is behind object
            float dot = Vector3.Dot(_constraint.transform.forward, (transform.position - _constraint.transform.position).normalized);
            _constraint.weight = Mathf.InverseLerp(dotRange.x, dotRange.y, dot);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if(Constraint && Target)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_constraint.transform.position, _constraint.transform.forward);

            Vector3 vecToTarget = (transform.position - _constraint.transform.position).normalized;
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_constraint.transform.position, vecToTarget);

            UnityEditor.Handles.Label(_constraint.transform.position, Vector3.Dot(_constraint.transform.forward, vecToTarget).ToString());
        }
    }
#endif
}
