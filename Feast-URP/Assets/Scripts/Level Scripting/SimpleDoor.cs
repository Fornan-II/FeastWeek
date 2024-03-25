using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    private const string k_ToggleTrigger = "ToggleDoor";

    [SerializeField] private Animator anim;
    [SerializeField] private Transform doorTransform;
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioCue.CueSettings openSoundSettings = AudioCue.CueSettings.Default;

    public void Interact()
    {
        anim.SetTrigger(k_ToggleTrigger);

        AudioManager.PlaySound(openSound, doorTransform, openSoundSettings);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!anim) anim = GetComponent<Animator>();
    }
#endif
}
