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
    [SerializeField] SpriteBlink spriteBlink;

    float playerHealth;
    float vignetteHurtBoost;

    void Update()
    {
        if (playerHealth > 0)
        {
            float vignetteWeight = 0;
            float healthFraction = playerHealth / maxPlayerHealth;
            if (healthFraction < 0.6f)
            {
                vignetteWeight += 1 - (healthFraction / 0.6f);
            }
            if (vignetteHurtBoost > 0)
            {
                vignetteHurtBoost = Mathf.Clamp(vignetteHurtBoost, 0, 1) * (1 - (0.4f * Time.deltaTime));
                vignetteWeight += vignetteHurtBoost * 5 * (1.5f - (healthFraction * 0.6f));
            }
            hurtVignette.weight = Mathf.Clamp(vignetteWeight, 0, Mathf.Max(1f, 1.333f - healthFraction));
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
        vignetteHurtBoost += (damage - (maxPlayerHealth / 10)) / maxPlayerHealth;
        spriteBlink.Blink(0.15f, color: Color.red);
        CameraMovement.Shake(0.1f);
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
