using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMechanic : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject doorBarrier;
    [SerializeField] private Transform doorCenterReference;
    [SerializeField] private ParticleSystem PassiveParticles;
    [Header("Explosion")]
    [SerializeField] private ParticleSystem ExplosionParticles;
    [SerializeField] private AudioClip doorExplosionSFX;
    [SerializeField] private AudioCue.CueSettings doorExplosionSFXSettings = AudioCue.CueSettings.Default;

    private List<LampPawn> _connectedLamps = new List<LampPawn>();
    private bool _doorHasOpened = false;

    public void AddConnectedLamp(LampPawn lamp) => _connectedLamps.Add(lamp);
    public void RemoveConnectedLamp(LampPawn lamp)
    {
        _connectedLamps.Remove(lamp);

        if(_connectedLamps.Count <= 0 && !_doorHasOpened)
        {
            OpenDoor();
        }
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

    public Vector3 GetTargetPosition() => doorCenterReference ? doorCenterReference.position : transform.position;

    [ContextMenu("Play door explosion sound")]
    private void PlayDoorExplosionSound() => AudioManager.PlaySound(doorExplosionSFX, GetTargetPosition(), doorExplosionSFXSettings);

    private void PlayDoorExplosion()
    {
        MainCamera.Effects.ApplyImpulse(transform.position, 0.25f);
        MainCamera.Effects.ApplyScreenShake(0.25f, 1f);
        MainCamera.Effects.ApplyScreenShake(0.1f, 5, 2f);

        PassiveParticles.Stop();
        ExplosionParticles.Play();

        StartCoroutine(FlashInvertedColor());
    }

    private IEnumerator FlashInvertedColor()
    {
        MainCamera.Effects.SetColorInvert(true);
        yield return new WaitForSecondsRealtime(0.1f);
        MainCamera.Effects.SetColorInvert(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetTargetPosition(), 0.25f);
    }
#endif
}
