using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventOnStart : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private float OnStartDelay = 0f;
    [SerializeField] private UnityEvent OnStart;
    [SerializeField] private UnityEvent OnEnabled;
    [SerializeField] private UnityEvent OnDisabled;
    [SerializeField] private UnityEvent OnDestroyed;
    
    private IEnumerator Start()
    {
        if(OnStartDelay > 0f)
        {
            yield return new WaitForSeconds(OnStartDelay);
        }

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
