using UnityEngine;
using UnityEngine.Events;

public class UnityEventTrigger : LimitedTrigger
{
    [SerializeField] private UnityEvent onTriggerEvent;

    protected override void OnTrigger() => onTriggerEvent.Invoke();
}
