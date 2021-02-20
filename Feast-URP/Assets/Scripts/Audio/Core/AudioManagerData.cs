using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioManager Data", menuName = "Data/New AudioManager Data")]
public class AudioManagerData : ScriptableObject
{
#pragma warning disable 0649
    [SerializeField] private AudioMixer mixer = null;
    public AudioMixer Mixer => mixer;

    [SerializeField] private AnimationCurve defaultRollOffCurve;
    public AnimationCurve DefaultRollOffCurve => defaultRollOffCurve;
}
