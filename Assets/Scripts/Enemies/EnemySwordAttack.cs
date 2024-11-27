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
    [SerializeField] GameObject hitParticle;

    GameObject newParticle;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            playersInsideTrigger.Add(collision);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playersInsideTrigger.Remove(collision);
        }
    }

    public void Attack()
    {
        AddKnockback();
        AddDamage();
        InstantiateHitParticle();
    }

    void AddKnockback()
    {
        for (int i = playersInsideTrigger.Count - 1; i >= 0; i--)
        {
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

    void InstantiateHitParticle()
    {
        newParticle = Instantiate(hitParticle, transform.position, Quaternion.identity);
        Invoke(nameof(DestroyParticle), 0.5f);
    }

    void DestroyParticle()
    {
        Destroy(newParticle);
    }
}
