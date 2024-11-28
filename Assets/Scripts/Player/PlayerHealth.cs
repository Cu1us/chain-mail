using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxPlayerHealth;
    [SerializeField] Image HealthBar;

    float playerHealth;

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
    }

    public void TakeDamage(float damage)
    {
        AudioManager.Play("hurtplayer");
        playerHealth -= damage;
        UpdateHealthBar();
        if(playerHealth <= 0)
        {
            Death();
        }
    }

    void UpdateHealthBar()
    {
        HealthBar.fillAmount =  1 - playerHealth/maxPlayerHealth;
    }

    void PlayerRevive()
    {
        playerHealth = maxPlayerHealth;
       // SendMessage(OnRevive);
    }

    void Death()
    {
      //  SendMessage(OnDeath);
    }
}
