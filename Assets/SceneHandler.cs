using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHandler : MonoBehaviour
{

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            FadeOut();
        }
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }

    public void LoadNextScene()
    {
        SceneLoadingManager.NextLevel();
    }
}
