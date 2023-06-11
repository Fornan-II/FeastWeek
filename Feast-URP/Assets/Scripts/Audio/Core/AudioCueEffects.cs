using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioCueEffects
{
    public static void Mix(AudioCue a, AudioCue b, float mix01)
    {
        mix01 *= mix01;
        a.SetVolume(Mathf.Lerp(0f, a.Settings.Volume, mix01), false);
        b.SetVolume(Mathf.Lerp(b.Settings.Volume, 0f, mix01), false);
    }
}
