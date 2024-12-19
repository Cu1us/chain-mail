using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxPlayerHealth;
    [SerializeField] GameObject gameoverText;
    [SerializeField] Image HealthBar;
    [SerializeField] PlayerInputData playerInput;
    [SerializeField] Volume hurtVignette;

    float playerHealth;
    float vignetteStrength;

    void Update()
    {
        if (vignetteStrength > 0)
        {
            hurtVignette.weight = vignetteStrength / (vignetteStrength + 1);
            vignetteStrength -= Time.deltaTime;
        }
        else
        {
            hurtVignette.weight = 0;
        }
    }
    void Start()
    {
        playerHealth = maxPlayerHealth;
    }

    public void TakeDamage(float damage)
    {
        AudioManager.Play("hurtplayer");
        playerHealth -= damage;
        vignetteStrength += damage;
        UpdateHealthBar();
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Death();
        }
    }

    void UpdateHealthBar()
    {
        HealthBar.fillAmount = 1 - playerHealth / maxPlayerHealth;
    }

    public void Death()
    {
        gameoverText.SetActive(true);
        playerInput.DisableInput();
        Invoke(nameof(ResetScene), 2);
        if (TryGetComponent(out Animator animator))
        {
            animator.SetBool("Dead", true);
        }
    }

    void ResetScene()
    {
        SceneLoadingManager.RestartLevel();
    }
}
