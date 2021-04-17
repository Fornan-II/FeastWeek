using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private AudioClip[] songs;
    public AudioSource musicSource;
    [SerializeField] private Vector2 timeBetweenSongs = new Vector2(10f, 60f);

    private float _timeUntilNextSong = -1;

    public void PlayImmediately()
    {
        _timeUntilNextSong = -1f;
        musicSource.Play();
    }

    public void PlayImmediately(int songIndex)
    {
        _timeUntilNextSong = -1;
        musicSource.clip = songs[songIndex];
        musicSource.Play();
    }

    public void FadeOut(float duration)
    {
        MainCamera.CameraData.StartCoroutine(FadeAudio(duration));
    }

    private IEnumerator FadeAudio(float duration)
    {
        float startingVolume = musicSource.volume;
        for (float timer = 0.0f; timer < duration; timer += Time.deltaTime)
        {
            yield return null;
            float t = timer / duration;
            musicSource.volume = Mathf.Lerp(startingVolume, 0f, t * t); ;
        }

        enabled = false;
        musicSource.Stop();
        musicSource.volume = startingVolume;
    }
    
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
