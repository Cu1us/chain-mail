using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    [Header("Settings")]
    [SerializeField] float knockbackForce;
    [SerializeField] float knockbackRotatingForce;

    [Header("References")]
    [SerializeField] PolygonCollider2D swordCollider;
    [SerializeField] BoxCollider2D swordRotateCollider;

    [SerializeField] Transform player1;
    [SerializeField] Transform player2;
    [SerializeField] Transform swordPivot;

    [SerializeField] Chain_Demo chain;
    [SerializeField] Animator animator;

    List<Collider2D> enemiesInsideTrigger = new List<Collider2D>();
    Dictionary<GameObject, float> enemies = new Dictionary<GameObject, float>();

    bool isChainRotating;

    void Start()
    {
        Application.targetFrameRate = 160; // Should later be removed!
    }

    void Update()
    {
        ChainRotating();

        if (Input.GetKeyDown(KeyCode.D))
        {
            ChangeColliderAngle(0);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeColliderAngle(180);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
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
                    Attack();
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

    void ChainRotating()
    {
        if (chain.grabStatus == Chain_Demo.GrabStatus.B)
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
    }

    public void Attack()
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
                Vector2 forceDirection = enemiesInsideTrigger[i].transform.position - transform.position;
                forceDirection.Normalize();
                enemiesInsideTrigger[i].GetComponent<Rigidbody2D>().AddForce(forceDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
        else
        {
            foreach (var enemy in enemiesInsideTrigger)
            {
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
        //Destroy(enemiesInsideTrigger[i].gameObject);
    }
}