using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    bool isGamePaused;
    [SerializeField] GameObject pauseMenu;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("EscPressed");
            if(isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

   public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Debug.Log("Resume");
        isGamePaused = false;
        TimeManager.timeScale = 1;
    }
    void PauseGame()
    {
        pauseMenu.SetActive(true);
        Debug.Log("Pause");
        isGamePaused = true;
        TimeManager.timeScale = 0;
    }

    public void ExitToMenu()
    {
        TimeManager.timeScale = 1;
        SceneLoadingManager.LoadLevel("Menu");
    }

    public void RestartLevel()
    {
        TimeManager.timeScale = 1;
        SceneLoadingManager.RestartLevel();
    }
}
