using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordAttack : MonoBehaviour
{
    [SerializeField] float knockback;
    [SerializeField] float damage;

    List<Collider2D> playersInsideTrigger = new List<Collider2D>();

    [SerializeField] EnemyMovement state;
    [SerializeField] Animator animator;
    [SerializeField] CollisionDetector collisionDetector;
    [SerializeField] Transform impactPos;
    [SerializeField] bool isSwordMan;

    GameObject newParticle;

    private void Start()
    {
        collisionDetector.onTriggerEnter += CollisionEnter;
        collisionDetector.onTriggerExit += CollisionExit;
    }

    void Update()
    {
        if (playersInsideTrigger.Count > 0 && state.state != EnemyMovement.EnemyState.STUCK)
        {
            animator.Play("Attack");
        }
    }

    void CollisionEnter(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playersInsideTrigger.Add(collision);
        }
    }

    void CollisionExit(Collider2D collision)
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
    }

    void AddKnockback()
    {
        for (int i = playersInsideTrigger.Count - 1; i >= 0; i--)
        {
            Vector2 forceDirection = playersInsideTrigger[i].transform.position - transform.position;
            forceDirection.Normalize();
            playersInsideTrigger[i].GetComponent<SwingableObject>().Launch(forceDirection * knockback);
        }
    }

    void AddDamage()
    {
        for (int i = 0; i < playersInsideTrigger.Count; i++)
        {
            if (playersInsideTrigger[i].TryGetComponent<PlayerHealth>(out PlayerHealth component))
            {
                playersInsideTrigger[i].GetComponent<PlayerHealth>().TakeDamage(damage);
                if(isSwordMan)
                {
                    AudioManager.Play("Hit");
                }
            }
        }
    }

    void InstantiateHitParticle()
    {
        VFX.Spawn(VFXType.GROUND_IMPACT, impactPos.position, 1);
    }

    public void SlegeHammerImpact()
    {
        AudioManager.Play("SledgeHammerImpact");
    }

    public void SwordSwing()
    {
        AudioManager.Play("swordswing");
    }
}
