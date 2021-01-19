using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioManager Data", menuName = "Data/New AudioManager Data")]
public class AudioManagerData : ScriptableObject
{
    [SerializeField] private AudioMixer mixer;
    public AudioMixer Mixer => mixer;

    [SerializeField] private AnimationCurve defaultRollOffCurve;
    public AnimationCurve DefaultRollOffCurve => defaultRollOffCurve;
}
