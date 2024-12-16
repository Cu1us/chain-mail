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
    [SerializeField] bool isSentinel;
    float sentinelWaitTimer;
    float sentinelChargeUpTime = 0.8f;
    bool sentinelChargeUp;
    GameObject newParticle;

    private void Start()
    {
        collisionDetector.onTriggerEnter += CollisionEnter;
        collisionDetector.onTriggerExit += CollisionExit;
    }

    void Update()
    {
        if(sentinelChargeUp && state.state != EnemyMovement.EnemyState.STUCK)
        {
            sentinelWaitTimer += Time.deltaTime;
            animator.SetBool("isReady", true);
            if(sentinelWaitTimer > sentinelChargeUpTime && playersInsideTrigger.Count > 0) //how long before he does attacks. instant after that time
            {
                animator.Play("Attack");
                sentinelWaitTimer = 0;
            }
            else if (sentinelWaitTimer > 2) //how long he stands and waits
            {
                sentinelWaitTimer = 0;
                sentinelChargeUp = false;
                animator.SetBool("isReady",false);
            }
        }
        else
        {
            sentinelChargeUp = false;
            sentinelWaitTimer = 0;
            animator.SetBool("isReady",false);
        }

        if (playersInsideTrigger.Count > 0 && state.state != EnemyMovement.EnemyState.STUCK && isSwordMan)
        {
            animator.Play("Attack");
        }
        
    }

    void CollisionEnter(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playersInsideTrigger.Add(collision);
            if(isSentinel)
            {
                sentinelChargeUp = true;
            }
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
