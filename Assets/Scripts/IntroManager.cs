using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] int sceneToLoadOnFinish;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer source)
    {
        SceneManager.LoadScene(sceneToLoadOnFinish);
    }

    void OnSkipIntro()
    {
        SceneManager.LoadScene(sceneToLoadOnFinish);
    }
}
