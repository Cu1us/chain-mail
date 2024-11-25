using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float knockbackForce;

    [Header("References")]
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] PlayerInputData playerInputData;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Chain chain;

    void Start()
    {
        playerInputData.onAttackPress += AttackPress;
    }
    void Update()
    {
        FlipSprite();
    }

    void FlipSprite()
    {
        if (playerInputData.movementInput.x > 0.3f)
        {
            spriteRenderer.flipX = false;
        }
        else if (playerInputData.movementInput.x < -0.3f)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void AttackPress()
    {
        Debug.Log("TEST!");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            
        }
    }
}
