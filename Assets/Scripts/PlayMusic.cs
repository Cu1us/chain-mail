using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    public static PlayMusic playMusic;

    [SerializeField] float maxVolume;
    [SerializeField] float fadeInSpeed;
    [SerializeField] float fadeOutspeed;
    [SerializeField] AudioSource musicSource;

    bool fadeInMusic;
    bool fadeOutMusic;

    void Awake()
    {
        if (playMusic != null && playMusic != this)
        {
            Destroy(gameObject);
        }
        else
        {
            playMusic = this;
            DontDestroyOnLoad(gameObject);
        }

        StartMusic();
    }

    void Update()
    {
        FadeInMusic();
        FadeOutMusic();
    }

    public void StartMusic()
    {
        fadeInMusic = true;
        fadeOutMusic = false;
        musicSource.volume = 0;
        musicSource.Play();
    }

    public void StopMusic()
    {
        fadeInMusic = false;
        fadeOutMusic = true;
    }

    void FadeInMusic()
    {
        if (fadeInMusic)
        {
            if (maxVolume > musicSource.volume)
            {
                musicSource.volume += Time.deltaTime * fadeInSpeed;
            }
        }
    }

    void FadeOutMusic()
    {
        if (fadeOutMusic)
        {
            if (maxVolume >= 0)
            {
                musicSource.volume -= Time.deltaTime * fadeOutspeed;
            }
        }
    }
}
