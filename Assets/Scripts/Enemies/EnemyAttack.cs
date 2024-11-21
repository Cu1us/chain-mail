using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] EnemySwordAttack weapon;
    Pathfinding state;
    EnemyAttackTrigger trigger;
    float attackTimer;
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite attack;
    [SerializeField] Sprite normal;
    void Start()
    {
        trigger = GetComponentInChildren<EnemyAttackTrigger>();
        state  = GetComponent<Pathfinding>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }


    void FixedUpdate()
    {
        if(state.attackState && trigger.isPlayerInRange)
        {
            spriteRenderer.sprite = attack;
            attackTimer += Time.deltaTime;
            if(attackTimer > 1)
            {
                weapon.Attack();
                attackTimer = 0;
            }
        }
        else
        {
            attackTimer = 0;
            if(state.state != Pathfinding.EnemyState.STUCK)
            {
                spriteRenderer.sprite = normal;
            }
         //   spriteRenderer.sprite = normal; //Remove this
        }
    }
}
