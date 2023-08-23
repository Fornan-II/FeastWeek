using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceMultiplierSetter : MonoBehaviour, ITriggerListener
{
#pragma warning disable 0649
    [SerializeReference] protected BaseTriggerVolume triggerVolume;
    [SerializeField] protected AmbiencePlayer[] targetAmbience;
    [SerializeField] protected Vector2 blendRemap = new Vector2(0f, 1f);

    protected int _cachedInstanceID;

    protected virtual void Start()
    {
        _cachedInstanceID = GetInstanceID();
    }

    protected virtual void OnEnable() => triggerVolume.AddListener(this);
    protected virtual void OnDisable() => triggerVolume.RemoveListener(this);

    public virtual void OnOverlapStart() { }

    public virtual void OnOverlap()
    {
        foreach(var ambience in targetAmbience)
        {
            ambience.BlendFactor.SetModifier(_cachedInstanceID, 1f - Mathf.Lerp(blendRemap.x, blendRemap.y, triggerVolume.BlendValue));
        }
    }

    public virtual void OnOverlapExit()
    {
        foreach (var ambience in targetAmbience)
        {
            ambience.BlendFactor.RemoveModifier(_cachedInstanceID);
        }
    }
}
