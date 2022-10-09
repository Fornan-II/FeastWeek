using System.Collections;
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
    [SerializeField] private AudioClip lampActiveSFX;
    [SerializeField] private AudioCue.CueSettings lampActiveSFXSettings = AudioCue.CueSettings.Default;
    [SerializeField] private AudioClip onLampAlignSFX;
    [SerializeField] private AudioCue.CueSettings onLampAlignSFXSettings = AudioCue.CueSettings.Default;
    [SerializeField] private float lampActiveFadeTime = 0.7f;
    [SerializeField] private AudioClip doorExplosionSFX;
    [SerializeField] private AudioCue.CueSettings doorExplosionSFXSettings = AudioCue.CueSettings.Default;

    [System.Serializable] struct LampData
    {
        public Transform Transform;
        public Vector2 SensitivityRange;
        public DoorParticles Particles;

        [HideInInspector] public AudioCue activeSFXCue;

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
        
        //bool anyLampFailed = false;
        //for(int i = 0; i < lamps.Length; ++i)
        //{
        //    float dot = Vector3.Dot(lamps[i].Transform.forward, (GetTargetPosition() - lamps[i].Transform.position).normalized);
        //    float tValue = Mathf.InverseLerp(lamps[i].SensitivityRange.x, lamps[i].SensitivityRange.y, dot);
        //    if (tValue < 0.5)                   // Lamp is not aligned correctly
        //    {
        //        anyLampFailed = true;

        //        if (lamps[i].activeSFXCue)
        //        {
        //            // Fade cue out then set inactive
        //            lamps[i].activeSFXCue.FadeOut(lampActiveFadeTime);
        //            lamps[i].activeSFXCue = null;
        //        }
        //    }
        //    else if (!lamps[i].activeSFXCue)    // Lamp is aligned correctly
        //    {
        //        // Start cue volume at zero then fade in
        //        lamps[i].activeSFXCue = AudioManager.PlaySound(lampActiveSFX, lamps[i].Transform, lampActiveSFXSettings);
        //        lamps[i].activeSFXCue.FadeIn(lampActiveSFXSettings.Volume, lampActiveFadeTime);

        //        // Play on align sfx
        //        AudioManager.PlaySound(onLampAlignSFX, GetTargetPosition(), onLampAlignSFXSettings);
        //    }

        //    runeDoor.sharedMaterial.SetFloat(lamps[i].GetMaterialPropertyID(), tValue);
        //    lamps[i].Particles.SensitiveActivate(tValue);
        //}

        //if (anyLampFailed) return;

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

    public Vector3 GetTargetPosition() => transform.position + doorCenterOffset;

    [ContextMenu("Play door explosion sound")]
    private void PlayDoorExplosionSound() => AudioManager.PlaySound(doorExplosionSFX, GetTargetPosition(), doorExplosionSFXSettings);

    private void PlayDoorExplosion()
    {
        for(int i = 0; i < lamps.Length; ++i)
        {
            lamps[i].Particles.Activate();
            if (lamps[i].activeSFXCue)
            {
                lamps[i].activeSFXCue.SetInactive();
                lamps[i].activeSFXCue = null;
            }
        }

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
        Gizmos.DrawSphere(GetTargetPosition(), 0.25f);
    }
#endif
}
