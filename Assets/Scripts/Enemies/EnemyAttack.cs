using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour

{
    Pathfinding state;
    EnemyAttackTrigger trigger;
    // Gameobject weapon;
    float attackTimer;
    void Start()
    {
        trigger = GetComponentInChildren<EnemyAttackTrigger>();
        state  = GetComponent<Pathfinding>();
        
    }


    void FixedUpdate()
    {
        if(state.attackState && trigger.isPlayerInRange)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer > 0.5)
            {
                //weapon.ATTACK();
                attackTimer = 0;
            }
        }
        else
        {
            attackTimer = 0;
        }
    }
}
