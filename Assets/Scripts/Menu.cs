using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneLoadingManager.LoadLevel(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
