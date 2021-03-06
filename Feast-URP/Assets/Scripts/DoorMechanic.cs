﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMechanic : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Animator animator;
    [SerializeField] private MeshRenderer runeDoor;
    [SerializeField] private LampData[] lamps;
    [SerializeField] private Vector3 doorCenterOffset;
    [SerializeField] private float doorDissolveTime = 2.0f;

    [System.Serializable] struct LampData
    {
        public Transform Transform;
        public Vector2 SensitivityRange;
        public DoorParticles Particles;
        [SerializeField] private string materialPropertyName;

        private int _materialPropertyID;
        private bool _materialPropertyIDInitialized;
        public int GetMaterialPropertyID()
        {
            if (!_materialPropertyIDInitialized)
            {
                _materialPropertyID = Shader.PropertyToID(materialPropertyName);
                _materialPropertyIDInitialized = true;
            }
            return _materialPropertyID;
        }
    }

    private bool _doorHasOpened = false;

    private void OnDestroy()
    {
        runeDoor.sharedMaterial.SetFloat("_Dissolve", 0f);
    }

    private void FixedUpdate()
    {
        if (_doorHasOpened) return;

        bool anyLampFailed = false;
        foreach (var lamp in lamps)
        {
            float dot = Vector3.Dot(lamp.Transform.forward, (transform.position + doorCenterOffset - lamp.Transform.position).normalized);
            float tValue = Mathf.InverseLerp(lamp.SensitivityRange.x, lamp.SensitivityRange.y, dot);
            if (tValue < 0.5)
                anyLampFailed = true;

            runeDoor.sharedMaterial.SetFloat(lamp.GetMaterialPropertyID(), tValue);
            lamp.Particles.SensitiveActivate(tValue);
        }

        if (anyLampFailed) return;

        OpenDoor();
    }

    public void OpenDoor()
    {
        animator.SetBool("IsOpen", true);
        _doorHasOpened = true;
    }

    public void CloseDoor()
    {
        animator.SetBool("IsOpen", false);
    }

    private void PlayDoorExplosion()
    {
        foreach (var lamp in lamps)
            lamp.Particles.Activate();
        StartCoroutine(DoorDissolve());
    }

    private void StopAllParticles()
    {
        foreach (var lamp in lamps)
            lamp.Particles.Deactivate();
    }

    private IEnumerator DoorDissolve()
    {
        int shaderID = Shader.PropertyToID("_Dissolve");
        for(float timer = 0.0f; timer < doorDissolveTime; timer += Time.deltaTime)
        {
            yield return null;
            runeDoor.sharedMaterial.SetFloat(shaderID, timer / doorDissolveTime);
        }
        runeDoor.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + doorCenterOffset, 0.25f);
    }

    /*private void OnGUI()
    {
        int lightSuccesses = 0;
        foreach(LampData lamp in lamps)
        {
            float dot = Vector3.Dot(lamp.Transform.forward, (transform.position + doorCenterOffset - lamp.Transform.position).normalized);
            float tValue = Mathf.InverseLerp(lamp.SensitivityRange.x, lamp.SensitivityRange.y, dot);
            if(tValue > 0)
            {
                GUI.Box(new Rect(0, lightSuccesses * 25f, 150f, 25f), Util.RoundToPlaces(dot, 5) + " : " + lamp.Transform.parent.name);
                ++lightSuccesses;
            }
        }
    }*/
#endif
}
