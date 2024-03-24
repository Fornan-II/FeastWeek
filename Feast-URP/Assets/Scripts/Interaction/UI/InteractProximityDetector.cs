using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractProximityDetector : MonoBehaviour
{
    [SerializeField] private InteractableIndicator indicatorUI;
    [SerializeField] private float radius = 3f;
    [SerializeField] private LayerMask layerMask = Physics.AllLayers;

    private Collider[] _overlapResults = new Collider[32];

    void FixedUpdate()
    {
        Interactable nearest = null;

        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, radius, _overlapResults, layerMask, QueryTriggerInteraction.Ignore);
        if(hitCount > 0)
        {
            float sqrDistanceToNearest = Mathf.Infinity;
            foreach(Collider c in _overlapResults)
            {
                if(c && c.TryGetComponent(out Interactable interactable))
                {
                    float sqrDistance = (c.transform.position - transform.position).sqrMagnitude;
                    if(sqrDistance < sqrDistanceToNearest)
                    {
                        nearest = interactable;
                        sqrDistanceToNearest = sqrDistance;
                    }
                }
            }
        }

        indicatorUI.SetInteractable(nearest);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
