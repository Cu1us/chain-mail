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
    [SerializeField] float sentinelWaitTimer;
    float chargeUpTimer;
    float sentinelChargeUpTime = 0.8f;
    float sentinelFollowUpAttackTime = 1f;
    bool sentinelChargeUp;
    GameObject newParticle;
    [SerializeField] bool playerInRange = false;
    Transform player;

    private void Start()
    {
        collisionDetector.onTriggerEnter += CollisionEnter;
        collisionDetector.onTriggerExit += CollisionExit;
        player = GameObject.Find("Player1").GetComponent<Transform>();
    }

    void Update()
    {
        if (isSentinel)
        {
            sentinelWaitTimer += Time.deltaTime;

            if (sentinelChargeUp && state.state != EnemyMovement.EnemyState.STUCK && state.state != EnemyMovement.EnemyState.DEAD && sentinelWaitTimer > 0)
            {
                chargeUpTimer += Time.deltaTime;
                //sentinelWaitTimer += Time.deltaTime;
                animator.SetBool("isReady", true);
                if (chargeUpTimer > sentinelChargeUpTime && playersInsideTrigger.Count > 0 && playerInRange)
                {
                    animator.Play("Attack");
                    sentinelWaitTimer = 0 - sentinelFollowUpAttackTime;
                    animator.SetBool("isReady", false);
                }
                else if (chargeUpTimer > 2) //Give this its own counter
                {
                    sentinelWaitTimer = 0;
                    chargeUpTimer = 0;
                    sentinelChargeUp = false;
                    animator.SetBool("isReady", false);
                }
            }
            else
            {
                chargeUpTimer = 0;
                animator.SetBool("isReady", false);
            }
        }


        if (playerInRange && state.state != EnemyMovement.EnemyState.STUCK && isSwordMan && state.state != EnemyMovement.EnemyState.DEAD) //playersInsideTrigger.Count > 0 
        {
            animator.Play("Attack");
        }

    }

    void CollisionEnter(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playersInsideTrigger.Add(collision);
            if (collision.transform.position == player.position)
            {
                playerInRange = true;
                sentinelChargeUp = true;
            }

            else if (state.state != EnemyMovement.EnemyState.STUCK)
            {
                state.StateChange(EnemyMovement.EnemyState.MOVECLOSETOATTACK);
            }
        }
    }

    void CollisionExit(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.transform.position == player.position)
            {
                playerInRange = false;
                sentinelChargeUp = false;
            }
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
                if (isSwordMan)
                {
                    AudioManager.Play("Hit");
                }
            }
            else if (playersInsideTrigger[i].TryGetComponent<BagTakeDamage>(out BagTakeDamage component1))
            {
                playersInsideTrigger[i].GetComponent<BagTakeDamage>().TakeDamage();
            }
        }
    }

    void InstantiateHitParticle()
    {
        VFX.Spawn(VFXType.GROUND_IMPACT, impactPos.position, 1.7f);
        VFX.Spawn(VFXType.CRACK, impactPos.position);
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
