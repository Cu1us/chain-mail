using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] EnemySwordAttack weapon;
    EnemyMovement state;
    EnemyAttackTrigger trigger;
    float attackTimer;
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite attack;
    [SerializeField] Sprite normal;
    [SerializeField] Sprite swing;
    void Start()
    {
        trigger = GetComponentInChildren<EnemyAttackTrigger>();
        state  = GetComponent<EnemyMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }


    void FixedUpdate()
    {
        if(state.isAttackState && trigger.isPlayerInRange)
        {
            spriteRenderer.sprite = attack;
            attackTimer += Time.deltaTime;
            if(attackTimer > 1)
            {
                spriteRenderer.sprite = swing;
                weapon.Attack();
                attackTimer = 0;
            }
        }
        else
        {
            attackTimer = 0;
            if(state.state != EnemyMovement.EnemyState.STUCK)
            {
                spriteRenderer.sprite = normal;
            }
         //   spriteRenderer.sprite = normal; //Remove this
        }
    }
}
