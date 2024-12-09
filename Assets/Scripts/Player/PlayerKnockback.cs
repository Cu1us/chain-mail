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


    [SerializeField] Chain.AnchorStatus anchorStatus;
    [SerializeField] bool freezeTimeOnHit;

    [Header("References")]
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] PlayerInputData playerInputData;
    [SerializeField] SwingableObject swingableObject;
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
        if (Mathf.Abs(swingableObject.swingVelocity) > 0 || swingableObject.beingSwapped)    
        {
            Debug.Log("TEEEEEST");
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
        if (swingableObject.beingSwapped)
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

            if (anchorStatus == Chain.AnchorStatus.ROCK)
            {
                forceDirection *= -1;
            }

            enemy.GetComponent<Rigidbody2D>().AddForce(forceDirection * swapKnockbackForce, ForceMode2D.Impulse);
        }
        else
        {
            Vector2 perpendicular = Vector2.Perpendicular(player1.position - player2.position);
            perpendicular *= Mathf.Sign(chain.rotationalVelocity);

            Vector2 enemyDirection = player1.position - player2.position;

            if (anchorStatus == Chain.AnchorStatus.ROCK)
            {
                perpendicular *= -1;
                enemyDirection *= -1;
            }

            Vector2 forceDirection = perpendicular + enemyDirection;
            forceDirection.Normalize();

            enemy.GetComponent<Rigidbody2D>().AddForce(forceDirection * rotateKnockbackForce, ForceMode2D.Impulse);
        }

        if (freezeTimeOnHit) TimeManager.Slowmo(0.1f, 0.2f);
        enemy.GetComponent<SpriteBlink>().Blink(0.2f);
    }

    void AddDamage(GameObject enemy)
    {
        float damage;

        if (swingableObject.beingSwapped)
        {
            damage = knockbackBaseDamage * swingableObject.velocity.magnitude * knockbackSwapDamageMultiply;
        }
        else
        {
            damage = knockbackBaseDamage * swingableObject.swingVelocity * knockbackSwingDamageMultiply;
        }

        enemy.GetComponent<EnemyHealth>().TakeDamage(Mathf.Abs(damage));
    }

    void ClearEnemies()
    {
        if (chain.anchorStatus == anchorStatus)
        {
            enemies.Clear();
        }
    }
}
