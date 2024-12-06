using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float swapKnockbackForce;
    [SerializeField] float rotateKnockbackForce;
    //[SerializeField] float maxAttackTime;
    [SerializeField] float coolDown;
    [SerializeField] float knockbackBaseDamage;
    [SerializeField] float knockbackSwingDamageMultiply;
    [SerializeField] float knockbackSwapDamageMultiply;


    [SerializeField] Chain.GrabStatus grabStatus;
    [SerializeField] bool freezeTimeOnHit;

    [Header("References")]
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] PlayerInputData playerInputData;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Chain chain;

    [SerializeField] Transform player1;
    [SerializeField] Transform player2;

    bool pressedAttack;
    float attackTimer;
    float coolDownTimer;

    Dictionary<GameObject, float> enemies = new Dictionary<GameObject, float>();

    // GIVES THE PLAYER ABILITY TO ADJUST THE KNOCKBACK DIRECTION
    /////////////////////////////////////////////////////////////////////
    /*void Start()
    {
        playerInputData.onAttackPress += AttackPress;
    }
    void Update()
    {
        AttackTimer();
    }

    void AttackPress()
    {
        pressedAttack = true;
    }

    void AttackTimer()
    {
        if (pressedAttack)
        {
            attackTimer += Time.deltaTime;
        }

        if (attackTimer > maxAttackTime)
        {
            pressedAttack = false;
            attackTimer = 0;
        }
    }*/
    /////////////////////////////////////////////////////////////////////

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (chain.grabStatus == grabStatus || playerMovement.beingSwapped)
        {
            if (collision.CompareTag("Enemy"))
            {
                float currentTime = Time.time;
                if (!enemies.ContainsKey(collision.gameObject) || currentTime - enemies[collision.gameObject] > coolDown)
                {
                    AddKnockback(collision.gameObject);
                    AddDamage(collision.gameObject);
                    enemies[collision.gameObject] = currentTime;
                }
            }
        }
    }

    void AddKnockback(GameObject enemy)
    {
        if (playerMovement.beingSwapped)
        {
            // GIVES THE PLAYER ABILITY TO ADJUST THE KNOCKBACK DIRECTION
            /////////////////////////////////////////////////////////////////////
            /*Vector2 forceDirection = playerInputData.movementInput.normalized;

            if (forceDirection == Vector2.zero)
            {
                forceDirection = player1.position - player2.position;
                forceDirection.Normalize();

                if (grabStatus == Chain.GrabStatus.A)
                {
                    forceDirection *= -1;
                }
            }*/
            /////////////////////////////////////////////////////////////////////

            Vector2 forceDirection = player1.position - player2.position;
            forceDirection.Normalize();

            if (grabStatus == Chain.GrabStatus.A)
            {
                forceDirection *= -1;
            }

            enemy.GetComponent<Rigidbody2D>().AddForce(forceDirection * swapKnockbackForce, ForceMode2D.Impulse);

            if (freezeTimeOnHit) TimeManager.Slowmo(0.1f, 0.2f);
            enemy.GetComponent<SpriteBlink>().Blink(0.2f);
        }
        else
        {
            Vector2 perpendicular = Vector2.Perpendicular(player1.position - player2.position);
            perpendicular *= Mathf.Sign(chain.rotationalVelocity);

            Vector2 enemyDirection = player1.position - player2.position;

            if (grabStatus == Chain.GrabStatus.A)
            {
                perpendicular *= -1;
                enemyDirection *= -1;
            }

            Vector2 forceDirection = perpendicular + enemyDirection;
            forceDirection.Normalize();

            enemy.GetComponent<Rigidbody2D>().AddForce(forceDirection * rotateKnockbackForce, ForceMode2D.Impulse);
        }
    }

    void AddDamage(GameObject enemy)
    {
        float damage;

        if (playerMovement.beingSwapped)
        {
            damage = knockbackBaseDamage * playerMovement.velocity.magnitude * knockbackSwapDamageMultiply;
        }
        else
        {
            damage = knockbackBaseDamage * playerMovement.swingVelocity * knockbackSwingDamageMultiply;
        }

        enemy.GetComponent<EnemyHealth>().TakeDamage(Mathf.Abs(damage));
    }

    void ClearEnemies()
    {
        if (chain.grabStatus == grabStatus)
        {
            enemies.Clear();
        }
    }
}
