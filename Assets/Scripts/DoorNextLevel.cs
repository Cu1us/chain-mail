using UnityEngine;
using UnityEngine.InputSystem;

public class DoorNextLevel : MonoBehaviour
{
    BoxCollider2D coll;
    Animator animator;
    TempSpawnEnemies spawner;
    public bool isLevelCleared = true;
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        coll.enabled = false;
        animator = GetComponent<Animator>();
        spawner = GameObject.Find("EnemySpawner").GetComponent<TempSpawnEnemies>();
    }

    public void OpenNextLevel()
    {

        coll.enabled = true;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Door");
        if (other.CompareTag("Player") && isLevelCleared)
        {
            animator.Play("DoorOpenAnimation");
        }
    }

    public void NextLevel()
    {
        GameObject.Find("Fade").GetComponent<SceneHandler>().FadeOut();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Keypad7))
        {
            SceneLoadingManager.LoadLevel("MainScene");
        }
        if (Input.GetKey(KeyCode.Keypad8))
        {
            SceneLoadingManager.LoadLevel("TutorialScene");
        }
    }
}
