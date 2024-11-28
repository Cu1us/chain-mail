using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxPlayerHealth;
    [SerializeField] GameObject gameoverText;
    [SerializeField] Image HealthBar;
    [SerializeField] PlayerInputData playerInput;

    float playerHealth;
    bool death;

    void Start()
    {
        playerHealth = maxPlayerHealth;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            PlayerRevive();
        }

        if (death)
        {
            Death();
        }
    }

    public void TakeDamage(float damage)
    {
        AudioManager.Play("hurtplayer");
        playerHealth -= damage;
        UpdateHealthBar();
        if(playerHealth <= 0)
        {
            death = true;
        }
    }

    void UpdateHealthBar()
    {
        HealthBar.fillAmount =  1 - playerHealth/maxPlayerHealth;
    }

    void PlayerRevive()
    {
        playerHealth = maxPlayerHealth;
    }

    void Death()
    {
        gameoverText.SetActive(true);
        playerInput.DisableInput();
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
