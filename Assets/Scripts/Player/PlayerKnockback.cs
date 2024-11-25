using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerKnockback : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float knockbackForce;
    [SerializeField] float normalKnockbackForce;
    [SerializeField] float maxAttackTime;
    [SerializeField] float coolDown;
    [SerializeField] Chain.GrabStatus grabStatus;

    [Header("References")]
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] PlayerInputData playerInputData;
    [SerializeField] Chain chain;

    [SerializeField] Transform player1;
    [SerializeField] Transform player2;

    bool pressedAttack;
    float attackTimer;
    float coolDownTimer;

    Dictionary<GameObject, float> enemies = new Dictionary<GameObject, float>();

    void Start()
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
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (chain.grabStatus == grabStatus)
        {
            if (collision.CompareTag("Enemy"))
            {
                float currentTime = Time.time;
                if (!enemies.ContainsKey(collision.gameObject) || currentTime - enemies[collision.gameObject] > coolDown)
                {
                    AddKnockback(collision.gameObject);
                    enemies[collision.gameObject] = currentTime;
                }
            }
        }
    }

    void AddKnockback(GameObject enemy)
    {
        if (enemy.TryGetComponent<Pathfinding>(out Pathfinding pathfinding))
        {
            pathfinding.CancelAgentUpdate();
        }

        if (pressedAttack)
        {
            Vector2 forceDirection = playerInputData.movementInput.normalized;
            enemy.GetComponent<Rigidbody2D>().AddForce(forceDirection * knockbackForce, ForceMode2D.Impulse);
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

            enemy.GetComponent<Rigidbody2D>().AddForce(forceDirection * normalKnockbackForce, ForceMode2D.Impulse);
        }
    }

    void ClearEnemies()
    {
        if (chain.grabStatus == grabStatus)
        {
            enemies.Clear();
        }
    }
}
