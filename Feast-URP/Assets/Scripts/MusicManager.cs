using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] songs;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Vector2 timeBetweenSongs = new Vector2(10f, 60f);

    private float _timeUntilNextSong = -1;
    
    // Update is called once per frame
    void Update()
    {
        if(!musicSource.isPlaying)
        {
            if(_timeUntilNextSong > 0f)
            {
                _timeUntilNextSong -= Time.deltaTime;
                if(_timeUntilNextSong <= 0)
                {
                    musicSource.clip = Util.RandomFromCollection(songs);
                    musicSource.Play();
                }
            }
            else
            {
                _timeUntilNextSong = Util.RandomInRange(timeBetweenSongs);
            }
        }
    }
}
