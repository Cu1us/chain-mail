using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoadingManager 
{

    public static void LoadLevel(string _scene)
    {
        SceneManager.LoadScene(_scene);
    }
    public static void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public static void NextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        try
        {
            SceneManager.LoadScene(nextSceneIndex);
        } 

        catch
        {
            SceneManager.LoadScene(0);
        }
    }
}
