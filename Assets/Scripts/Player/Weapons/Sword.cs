using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Sword : Weapon
{
    [Header("Settings")]
    [SerializeField] float knockbackForce;
    [SerializeField] float knockbackRotatingForce;
    [SerializeField] float damage;

    [Header("References")]
    [SerializeField] PolygonCollider2D swordCollider;
    [SerializeField] BoxCollider2D swordRotateCollider;

    [SerializeField] Transform player1;
    [SerializeField] Transform player2;
    [SerializeField] Transform swordPivot;

    [SerializeField] Chain chain;
    [SerializeField] PlayerInputData playerInputData;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;

    [SerializeField] GameObject bloodParticle;

    List<Collider2D> enemiesInsideTrigger = new List<Collider2D>();
    Dictionary<GameObject, float> enemies = new Dictionary<GameObject, float>();

    GameObject newBloodParticle;
    bool isChainRotating;

    void Start()
    {
        Application.targetFrameRate = 160; // Should later be removed!
    }

    void Update()
    {
        ChainRotating();
        FlipSprite();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemiesInsideTrigger.Add(collision);

            if (isChainRotating)
            {
                float currentTime = Time.time;
                if (!enemies.ContainsKey(collision.gameObject) || currentTime - enemies[collision.gameObject] > 0.5f)
                {
                    AttackPress();
                    enemies[collision.gameObject] = currentTime;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemiesInsideTrigger.Remove(collision);
        }
    }

    void FlipSprite()
    {
        if (playerInputData.movementInput.x > 0.3f)
        {
            ChangeColliderAngle(0);
        }
        else if (playerInputData.movementInput.x < -0.3f)
        {
            ChangeColliderAngle(180);
        }
    }

    void ChainRotating()
    {
        if (chain.grabStatus == Chain.GrabStatus.B)
        {
            isChainRotating = true;
            swordCollider.enabled = false;
            swordRotateCollider.enabled = true;

            Vector2 ChainDirection = player1.transform.position - player2.transform.position;
            ChainDirection.Normalize();
            float angleDegrees = (Mathf.Atan2(ChainDirection.y, ChainDirection.x) * Mathf.Rad2Deg);
            ChangeColliderAngle(angleDegrees);
        }
        else
        {
            if (isChainRotating) // Corrects the angle of the collider after chainrotation
            {
                if (player1.transform.position.x > player2.transform.position.x)
                {
                    ChangeColliderAngle(0);
                }
                else
                {
                    ChangeColliderAngle(180);
                }
            }
            enemies.Clear();
            isChainRotating = false;

            swordCollider.enabled = true;
            swordRotateCollider.enabled = false;
        }
    }

    void ChangeColliderAngle(float angle)
    {
        swordPivot.transform.rotation = Quaternion.Euler(0, 0, angle);
        if (angle > 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    public override void AttackPress()
    {
        animator.SetTrigger("PlayAttack");
        AddKnockback();
        AddDamage();
    }

    void AddKnockback()
    {
        if (!isChainRotating)
        {
            for (int i = enemiesInsideTrigger.Count - 1; i >= 0; i--)
            {
                if (enemiesInsideTrigger[i].TryGetComponent<Pathfinding>(out Pathfinding pathfinding))
                {
                    pathfinding.CancelAgentUpdate();
                }

                Vector2 forceDirection = enemiesInsideTrigger[i].transform.position - transform.position;
                forceDirection.Normalize();
                enemiesInsideTrigger[i].GetComponent<Rigidbody2D>().AddForce(forceDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
        else
        {
            foreach (var enemy in enemiesInsideTrigger)
            {
                if (enemy.TryGetComponent<Pathfinding>(out Pathfinding pathfinding))
                {
                    pathfinding.CancelAgentUpdate();
                }

                Vector2 perpendicular = Vector2.Perpendicular(player1.position - player2.position);
                perpendicular *= Mathf.Sign(chain.rotationalVelocity);
                Vector2 enemyDirection = player1.position - player2.position;
                Vector2 forceDirection = perpendicular + enemyDirection;
                forceDirection.Normalize();

                enemy.GetComponent<Rigidbody2D>().AddForce(forceDirection * knockbackForce * knockbackRotatingForce, ForceMode2D.Impulse);
            }
        }
    }

    void AddDamage()
    {
        foreach (var enemy in enemiesInsideTrigger)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(damage);
            InstantiateParticle(enemy.gameObject);
        }
    }

    void InstantiateParticle(GameObject enemy)
    {
        newBloodParticle = Instantiate(bloodParticle, transform.position, Quaternion.identity);
        newBloodParticle.transform.position = enemy.transform.position;
        newBloodParticle.transform.localScale = Vector3.one;
        Invoke(nameof(DestroyBloodParticle), 3);
    }

    void DestroyBloodParticle()
    {
        Destroy(bloodParticle);
    }

    public override void AttackRelease()
    {

    }
}
