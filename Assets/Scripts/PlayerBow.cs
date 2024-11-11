using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBow : MonoBehaviour
{
    public GameObject arrow;
    [SerializeField] float bowCooldown;

    float bowTimer;

    void Update()
    {
        bowTimer += Time.deltaTime;
        PlayerInput();
    }

    void PlayerInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ShootArrow();
        }
    }

    void ShootArrow()
    {
        if (bowTimer > bowCooldown)
        {
            Instantiate(arrow, transform.position, transform.rotation);
            bowTimer = 0;
        }
    }
}
