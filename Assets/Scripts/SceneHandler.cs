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

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }

    public void LoadNextScene()
    {
        SceneLoadingManager.NextLevel();
    }
}
