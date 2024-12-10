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
        float MaxBuildIndex = SceneManager.sceneCountInBuildSettings - 1;
       if(nextSceneIndex > MaxBuildIndex)
       {
        SceneManager.LoadSceneAsync(0);
       }
       else 
       {
        SceneManager.LoadScene(nextSceneIndex);
       }


    }
}
