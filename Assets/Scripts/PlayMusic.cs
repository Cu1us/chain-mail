using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    private static PlayMusic instance;

    [SerializeField] float maxVolume;
    [SerializeField] float fadeInSpeed;
    [SerializeField] AudioSource musicSource;

    bool fadeInMusic;
    bool fadeOutMusic;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        StartMusic();
    }

    void Update()
    {
        FadeInMusic();
    }

    void StartMusic()
    {
        fadeInMusic = true;
        musicSource.volume = 0;
        musicSource.Play();
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
                musicSource.volume -= Time.deltaTime * fadeInSpeed;
            }
        }
    }
}
