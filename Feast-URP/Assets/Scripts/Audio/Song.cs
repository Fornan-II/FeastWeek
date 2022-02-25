using System;
using System.Collections.Generic;

[Serializable]
public class Song
{
    public AudioCue SongCue;
    public List<SongEvent> SongEventQueue; // Using a list so that I can sort through them on insertion

    [Serializable]
    public struct SongEvent
    {
        public float Time;
        public Action Event;
    }

    public Song(AudioCue songCue)
    {
        SongCue = songCue;
        SongEventQueue = new List<SongEvent>();
    }

    public void AddSongEvent(float time, Action songEvent)
    {
        int i = 0;
        for (; i < SongEventQueue.Count; ++i)
        {
            if (time < SongEventQueue[i].Time)
                break;
        }
        SongEventQueue.Insert(i, new SongEvent() { Time = time, Event = songEvent });
    }

    public void CheckForSongEvent()
    {
        while (SongEventQueue.Count > 0 && SongCue.Source.time >= SongEventQueue[0].Time)
        {
            SongEventQueue[0].Event();
            SongEventQueue.RemoveAt(0);
        }
    }
}
