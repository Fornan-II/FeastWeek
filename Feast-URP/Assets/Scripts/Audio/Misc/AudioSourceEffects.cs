using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioSourceEffects
{
    public static void Mix(AudioSource a, float aVolume, AudioSource b, float bVolume, float mix01)
    {
        mix01 *= mix01;
        a.volume = Mathf.Lerp(0f, aVolume, mix01);
        b.volume = Mathf.Lerp(bVolume, 0f, mix01);
    }
}
