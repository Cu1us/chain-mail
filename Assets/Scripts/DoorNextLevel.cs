using UnityEngine;
using UnityEngine.InputSystem;

public class DoorNextLevel : MonoBehaviour
{
    BoxCollider2D coll;
    Animator animator;
    [SerializeField] bool isDoor;
    [SerializeField] bool isNoAnimation;

    public bool isLevelCleared = true;
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        coll.enabled = false;
        animator = GetComponent<Animator>();
    }

    public void OpenNextLevel()
    {
        coll.enabled = true;
        PlayMusic.StopMusic();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isLevelCleared)
        {
            if (isNoAnimation)
            {
                NextLevel();
            }
            else
            {
                animator.Play("DoorOpenAnimation");
            }

        }
    }

    public void NextLevel()
    {
        GameObject.Find("Fade").GetComponent<SceneHandler>().FadeOut();
    }

    // void Update()
    // {
    //     if (Input.GetKey(KeyCode.Keypad7))
    //     {
    //         SceneLoadingManager.LoadLevel("MainScene");
    //     }
    //     if (Input.GetKey(KeyCode.Keypad8))
    //     {
    //         SceneLoadingManager.LoadLevel("TutorialScene");
    //     }
    // }

}
