using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float playerHealth;

    void Start()
    {

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
        playerHealth -= damage;
        if(playerHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
      //  SendMessage(OnDeath);
    }

    void PlayerRevive()
    {
       // SendMessage(OnRevive);
    }

}
