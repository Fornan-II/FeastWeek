using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMechanic : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Animator animator;
    [SerializeField] private MeshRenderer runeDoor;
    [SerializeField] private Vector3 doorCenterOffset;
    [SerializeField] private float doorDissolveTime = 2.0f;
    [SerializeField] private AudioClip doorExplosionSFX;
    [SerializeField] private AudioCue.CueSettings doorExplosionSFXSettings = AudioCue.CueSettings.Default;

    private List<LampPawn> _connectedLamps = new List<LampPawn>();
    private bool _doorHasOpened = false;

    private void OnDestroy()
    {
        runeDoor.sharedMaterial.SetFloat("_Dissolve", 0f);
    }

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

    public Vector3 GetTargetPosition() => transform.position + doorCenterOffset;

    [ContextMenu("Play door explosion sound")]
    private void PlayDoorExplosionSound() => AudioManager.PlaySound(doorExplosionSFX, GetTargetPosition(), doorExplosionSFXSettings);

    private void PlayDoorExplosion()
    {
        MainCamera.Effects.ApplyImpulse(transform.position, 0.25f);
        MainCamera.Effects.ApplyScreenShake(0.25f);
        StartCoroutine(DoorDissolve());
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
