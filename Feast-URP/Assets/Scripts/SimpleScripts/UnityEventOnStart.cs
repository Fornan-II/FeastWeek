using UnityEngine;
using UnityEngine.Events;

public class UnityEventOnStart : MonoBehaviour
{
    [SerializeField] private UnityEvent OnStart;
    [SerializeField] private UnityEvent OnEnabled;
    [SerializeField] private UnityEvent OnDisabled;
    [SerializeField] private UnityEvent OnDestroyed;
    
    private void Start()
    {
        OnStart.Invoke();
    }

    private void OnEnable()
    {
        OnEnabled?.Invoke();
    }

    private void OnDisable()
    {
        OnDisabled?.Invoke();
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}
