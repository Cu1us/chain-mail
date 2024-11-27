using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordAttack : MonoBehaviour
{
    [SerializeField] float knockback;
    [SerializeField] float damage;
    [SerializeField] float attackChargeup;

    List<Collider2D> playersInsideTrigger = new List<Collider2D>();

    [SerializeField] EnemyMovement state;

    float attackTimer;

    void Update()
    {
        if (playersInsideTrigger.Count > 0)
        {
            attackTimer += Time.deltaTime;

            if (state.isAttackState && attackTimer > attackChargeup)
            {
                Attack();
                attackTimer = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            playersInsideTrigger.Add(collision);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            playersInsideTrigger.Remove(collision);
        }
    }

    public void Attack()
    {
        AddKnockback();
        AddDamage();
    }

    void AddKnockback()
    {
        for (int i = playersInsideTrigger.Count - 1; i >= 0; i--)
        {
            Debug.Log("ATTACKING PLAYER");
            Vector2 forceDirection = playersInsideTrigger[i].transform.position - transform.position;
            forceDirection.Normalize();
            playersInsideTrigger[i].GetComponent<PlayerMovement>().Launch(forceDirection * knockback);
        }
    }

    void AddDamage()
    {
        for (int i = 0; i < playersInsideTrigger.Count; i++)
        {
            playersInsideTrigger[i].GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }

    /*void RotateTowardsPlayer()
    {
    float targetAngle = transform.position.x - pathfinding.targetTransform.transform.position.x;

    if (targetAngle > 0)
    {
        targetAngle = 180;
    }
    else
    {
        targetAngle = 0;
    }

    transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }*/
}
