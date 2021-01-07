using UnityEngine;
using UnityEngine.Events;

public abstract class LimitedTrigger : MonoBehaviour
{
    [SerializeField] private bool onlyTriggerIfPlayer = true;
    [SerializeField] private int maxTimesCanBeTriggered = 1;

    private int timesTriggered = 0;

    private void OnDisable() => timesTriggered = 0;

    private void OnTriggerEnter(Collider other)
    {
        ++timesTriggered;
        if ((maxTimesCanBeTriggered <= 0 || timesTriggered <= maxTimesCanBeTriggered)
            && ((onlyTriggerIfPlayer && other.transform == PlayerRef.Transform)
            || !onlyTriggerIfPlayer))
        {
            OnTrigger();
        }
    }

    protected abstract void OnTrigger();
}
