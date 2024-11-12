using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    void Update()
    {
        PlayerInput();
    }

    void PlayerInput()
    {

    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.Space) && collision.tag == "Enemy")
        {
            Destroy(collision.gameObject);
        }
    }
}
